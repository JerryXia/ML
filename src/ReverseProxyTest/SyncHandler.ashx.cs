/*
 * REVERSE PROXY SERVER - IIS HTTP HANDLER - C# .NET v2.0
 * 
 * FILE NAME    :	Program.cs
 * 
 * DATE CREATED :	March 26, 2007, 16:15:05
 * CREATED BY   :	Gunasekaran Paramesh
 * 
 * LAST UPDATED :	April 16, 2007, 3:10:09 PM
 * UPDATED BY   :	Gunasekaran Paramesh
 * 
 * DESCRIPTION  :	Implementation of Reverse Proxy Server through IIS HTTP handler in C# .NET v2.0
*/

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Web.SessionState;

namespace ReverseProxyTest
{
    public class SyncHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext Context)
        {
            string ServerURL = "";

            try
            {
                // Parsing incoming URL and extracting original server URL
                char[] URL_Separator = { '/' };
                string[] URL_List = Context.Request.Url.AbsoluteUri.Remove(0, 7).Split(URL_Separator);
                ServerURL = "http://" + URL_List[2].Remove(URL_List[2].Length - 5, 5) + @"/";
                string URLPrefix = @"/" + URL_List[1] + @"/" + URL_List[2]; // Eg. "/handler/stg2web.sync";
                for (int i = 3; i < URL_List.Length; i++)
                    ServerURL += URL_List[i] + @"/";
                ServerURL = ServerURL.Remove(ServerURL.Length - 1, 1);
                WriteLog(ServerURL + " (" + Context.Request.Url.ToString() + ")");

                // Extracting POST data from incoming request
                Stream RequestStream = Context.Request.InputStream;
                byte[] PostData = new byte[Context.Request.InputStream.Length];
                RequestStream.Read(PostData, 0, (int)Context.Request.InputStream.Length);

                // Creating proxy web request
                HttpWebRequest ProxyRequest = (HttpWebRequest)WebRequest.Create(ServerURL);
                if (ConfigurationManager.AppSettings["UpchainProxy"] == "true")
                    ProxyRequest.Proxy = new WebProxy(ConfigurationManager.AppSettings["Proxy"], true);

                ProxyRequest.Method = Context.Request.HttpMethod;
                ProxyRequest.UserAgent = Context.Request.UserAgent;
                CookieContainer ProxyCookieContainer = new CookieContainer();
                ProxyRequest.CookieContainer = new CookieContainer();
                ProxyRequest.CookieContainer.Add(ProxyCookieContainer.GetCookies(new Uri(ServerURL)));
                ProxyRequest.KeepAlive = true;

                //For POST, write the post data extracted from the incoming request
                if (ProxyRequest.Method == "POST")
                {
                    ProxyRequest.ContentType = "application/x-www-form-urlencoded";
                    ProxyRequest.ContentLength = PostData.Length;
                    Stream ProxyRequestStream = ProxyRequest.GetRequestStream();
                    ProxyRequestStream.Write(PostData, 0, PostData.Length);
                    ProxyRequestStream.Close();
                }

                // Getting response from the proxy request
                HttpWebResponse ProxyResponse = (HttpWebResponse)ProxyRequest.GetResponse();
                if (ProxyRequest.HaveResponse)
                {
                    // Handle cookies
                    foreach (Cookie ReturnCookie in ProxyResponse.Cookies)
                    {
                        bool CookieFound = false;
                        foreach (Cookie OldCookie in ProxyCookieContainer.GetCookies(new Uri(ServerURL)))
                        {
                            if (ReturnCookie.Name.Equals(OldCookie.Name))
                            {
                                OldCookie.Value = ReturnCookie.Value;
                                CookieFound = true;
                            }
                        }
                        if (!CookieFound)
                            ProxyCookieContainer.Add(ReturnCookie);
                    }
                }

                Stream StreamResponse = ProxyResponse.GetResponseStream();
                int ResponseReadBufferSize = 256;
                byte[] ResponseReadBuffer = new byte[ResponseReadBufferSize];
                MemoryStream MemoryStreamResponse = new MemoryStream();

                int ResponseCount = StreamResponse.Read(ResponseReadBuffer, 0, ResponseReadBufferSize);
                while (ResponseCount > 0)
                {
                    MemoryStreamResponse.Write(ResponseReadBuffer, 0, ResponseCount);
                    ResponseCount = StreamResponse.Read(ResponseReadBuffer, 0, ResponseReadBufferSize);
                }

                byte[] ResponseData = MemoryStreamResponse.ToArray();
                string ResponseDataString = Encoding.ASCII.GetString(ResponseData);

                // While rendering HTML, parse and modify the URLs present
                if (ProxyResponse.ContentType.StartsWith("text/html"))
                {
                    HTML.ParseHTML Parser = new HTML.ParseHTML();
                    Parser.Source = ResponseDataString;
                    while (!Parser.Eof())
                    {
                        char ch = Parser.Parse();
                        if (ch == 0)
                        {
                            HTML.AttributeList Tag = Parser.GetTag();
                            if (Tag["href"] != null)
                            {
                                if (Tag["href"].Value.StartsWith(@"/"))
                                {
                                    WriteLog("URL " + Tag["href"].Value + " modified to " + URLPrefix + Tag["href"].Value);
                                    ResponseDataString = ResponseDataString.Replace("\"" + Tag["href"].Value + "\"", "\"" + URLPrefix + Tag["href"].Value + "\"");
                                }
                            }

                            if (Tag["src"] != null)
                            {
                                if (Tag["src"].Value.StartsWith(@"/"))
                                {
                                    WriteLog("URL " + Tag["src"].Value + " modified to " + URLPrefix + Tag["src"].Value);
                                    ResponseDataString = ResponseDataString.Replace("\"" + Tag["src"].Value + "\"", "\"" + URLPrefix + Tag["src"].Value + "\"");
                                }
                            }
                        }
                    }
                    Context.Response.Write(ResponseDataString);
                }
                else
                    Context.Response.OutputStream.Write(ResponseData, 0, ResponseData.Length);

                MemoryStreamResponse.Close();
                StreamResponse.Close();
                ProxyResponse.Close();
            }
            catch (Exception Ex)
            {
                Context.Response.Write(Ex.Message.ToString());
                WriteLog("An error has occured while requesting the URL " + ServerURL + "(" + Context.Request.Url.ToString() + ")\n" + Ex.ToString());
            }
        }


        private void WriteLog(string Message)
        {
            FileStream FS = new FileStream(ConfigurationManager.AppSettings["Log"], FileMode.Append, FileAccess.Write);
            string DateTimeString = DateTime.Now.ToString();
            Message = "[" + DateTimeString + "] " + Message + "\n";
            byte[] FileBuffer = Encoding.UTF8.GetBytes(Message);
            FS.Write(FileBuffer, 0, (int)FileBuffer.Length);
            FS.Flush();
            FS.Close();
        }
    }
}