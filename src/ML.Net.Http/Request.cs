using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace ML.Net.Http
{
    public class Request
    {
        #region Fields

        //HttpWebRequest对象用来发起请求
        private HttpWebRequest request = null;

        #endregion

        #region Public Func

        public void Send(RequestState requestState)
        {
            if (requestState == null)
            {
                return;
            }

            try
            {
                RequestSetting setting = requestState.Setting;
                SetHttpRequest(setting);

                requestState.Request = request;
                switch (setting.Method)
                {
                    case RequestMethod.POST:
                    case RequestMethod.PUT:
                    case RequestMethod.PATCH:
                        IAsyncResult getRequestResult = request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), requestState);
                        break;
                    case RequestMethod.OPTIONS:
                    case RequestMethod.HEAD:
                    case RequestMethod.GET:
                    case RequestMethod.DELETE:
                    case RequestMethod.TRACE:
                    case RequestMethod.CONNECT:
                    default:
                        IAsyncResult getResponseResult = request.BeginGetResponse(new AsyncCallback(GetResponseCallback), requestState);
                        break;
                }
            }
            catch(WebException webEx)
            {
                RaiseRequestException(requestState, webEx, null);
            }
            catch(Exception ex)
            {
                RaiseRequestException(requestState, null, ex);
            }
        }

        #endregion

        #region IOCP

        private void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        private void GetRequestStreamCallback(IAsyncResult result)
        {
            ThreadPoolMessage("BeginGetRequestStream Complete");

            RequestState requestState = result.AsyncState as RequestState;
            if(requestState != null)
            {
                try
                {
                    Stream postStream = requestState.Request.EndGetRequestStream(result);
                    postStream.Write(requestState.Setting.PostData, 0, requestState.Setting.PostData.Length);
                    postStream.Close();

                    requestState.Request.BeginGetResponse(new AsyncCallback(GetResponseCallback), requestState);
                }
                catch(Exception ex)
                {
                    RaiseRequestException(requestState, null, ex);
                }
            }
        }


        private void GetResponseCallback(IAsyncResult result)
        {
            ThreadPoolMessage("BeginGetResponse Complete");

            RequestState requestState = result.AsyncState as RequestState;
            if (requestState != null)
            {
                try
                {
                    requestState.HttpWebResponse = (HttpWebResponse)requestState.Request.EndGetResponse(result);
                    requestState.ResponseStream = requestState.HttpWebResponse.GetResponseStream();
                    requestState.ContentType = requestState.HttpWebResponse.ContentType;
                    // 获取响应头
                    //for (int i = 0; i < requestState.response.Headers.Count; i++)
                    //{
                    //    string key = requestState.response.Headers.GetKey(i);
                    //    string value = requestState.response.Headers.Get(i);
                    //    requestState.responseHeaders.Add(new KeyValuePair<string, string>(key, value));
                    //}

                    IAsyncResult readResult = requestState.ResponseStream.BeginRead(requestState.BufferRead,
                        0, RequestState.BUFFER_SIZE, new AsyncCallback(ReadCallBack), requestState);
                }
                catch(Exception ex)
                {
                    RaiseRequestException(requestState, null, ex);
                }
            }
        }

        private void ReadCallBack(IAsyncResult result)
        {
            ThreadPoolMessage("BeginRead Complete");

            RequestState requestState = result.AsyncState as RequestState;
            if (requestState != null)
            {
                try
                {
                    int read = requestState.ResponseStream.EndRead(result);
                    if (read > 0)
                    {
                        requestState.ResponseMemoryStream.Write(requestState.BufferRead, 0, read);
                        IAsyncResult readResult = requestState.ResponseStream.BeginRead(requestState.BufferRead,
                            0, RequestState.BUFFER_SIZE, new AsyncCallback(ReadCallBack), requestState);
                    }
                    else
                    {
                        requestState.ResponseStream.Close();
                        RaiseRequestOk(requestState);
                    }
                }
                catch(Exception ex)
                {
                    RaiseRequestException(requestState, null, ex);
                }
            }
        }

        private void RaiseRequestException(RequestState requestState, WebException webEx, Exception ex)
        {
            RequestCompeleteEventArgs e = new RequestCompeleteEventArgs(true);
            if(webEx != null)
            {
                e.WebError = webEx;
            }
            if (ex != null)
            {
                e.ExtraError = ex;
            }

            requestState.RaiseRequestCompelete(requestState, e);
        }

        private void RaiseRequestOk(RequestState requestState)
        {
            Response response = new Response(requestState.HttpWebResponse.StatusCode, requestState.HttpWebResponse.StatusDescription,
                requestState.HttpWebResponse.Headers, requestState.ResponseMemoryStream.ToArray());

            requestState.RaiseRequestCompelete(requestState, new RequestCompeleteEventArgs(response));
        }



        private void ThreadPoolMessage(string data)
        {
            int a, b;
            ThreadPool.GetAvailableThreads(out a, out b);
            string message = string.Format("{0}, CurrentThreadId is:{1}, WorkerThreads is:{2}, CompletionPortThreads is:{3}",
                  data, Thread.CurrentThread.ManagedThreadId, a.ToString(), b.ToString());

            Output(message);

        }

        private void Output(string message)
        {
            Trace.WriteLine(message);
        }

        #endregion

        #region Private Func

        void SetHttpRequest(RequestSetting setting)
        {
            InitHttpRequest(setting.Url);

            // 验证证书
            SetCerList(setting.ClientCertificates);
            SetCer(setting.CerPath);

            // 设置代理
            SetProxy(setting.ProxyIp, setting.ProxyPort, setting.ProxyUserName, setting.ProxyPwd);

            //设置RequestMethod
            SetMethod(setting.Method);
            request.ProtocolVersion = setting.ProtocolVersion;

            request.Accept = setting.Accept;
            request.UserAgent = setting.UserAgent;
            request.Referer = setting.Referer;

            //设置Cookie
            SetCookie(setting.Cookie, setting.CookieCollection);
            //设置自定义头
            SetHeaders(setting.Headers);


            request.ReadWriteTimeout = setting.ReadWriteTimeout;
            request.Timeout = setting.Timeout;
            request.AllowAutoRedirect = setting.AllowAutoRedirect;

            SetContentType(setting.ContentType);
        }


        void InitHttpRequest(string requestUriString)
        {
            //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = (HttpWebRequest)WebRequest.Create(requestUriString);
        }
        bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        void SetCerList(X509CertificateCollection clientCertificates)
        {
            if (clientCertificates != null && clientCertificates.Count > 0)
            {
                foreach (X509Certificate item in clientCertificates)
                {
                    request.ClientCertificates.Add(item);
                }
            }
        }
        void SetCer(string cerPath)
        {
            if (!String.IsNullOrEmpty(cerPath))
            {
                if (File.Exists(cerPath))
                {
                    //将证书添加到请求里
                    X509Certificate x509 = new X509Certificate(cerPath);
                    request.ClientCertificates.Add(x509);
                }
            }
        }

        void SetProxy(string ip, int port, string userName, string passWord)
        {
            if (!String.IsNullOrEmpty(ip))
            {
                WebProxy myProxy = new WebProxy(ip, port);
                myProxy.Credentials = new NetworkCredential(userName, passWord);
                request.Proxy = myProxy;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
        }

        void SetMethod(RequestMethod method)
        {
            string returnMethod = null;
            switch (method)
            {
                case RequestMethod.OPTIONS:
                    returnMethod = "OPTIONS";
                    break;
                case RequestMethod.HEAD:
                    returnMethod = "HEAD";
                    break;
                case RequestMethod.GET:
                    returnMethod = "GET";
                    break;
                case RequestMethod.POST:
                    returnMethod = "POST";
                    break;
                case RequestMethod.PUT:
                    returnMethod = "PUT";
                    break;
                case RequestMethod.DELETE:
                    returnMethod = "DELETE";
                    break;
                case RequestMethod.TRACE:
                    returnMethod = "TRACE";
                    break;
                case RequestMethod.CONNECT:
                    returnMethod = "CONNECT";
                    break;
                case RequestMethod.PATCH:
                    returnMethod = "PATCH";
                    break;
                default:
                    returnMethod = "GET";
                    break;
            }
            request.Method = returnMethod;
        }

        void SetCookie(string cookie, CookieCollection cookieCollection)
        {
            if (!string.IsNullOrEmpty(cookie))
            {
                request.Headers.Set(HttpRequestHeader.Cookie, cookie);
            }

            if (cookieCollection != null && cookieCollection.Count > 0)
            {
                if(request.CookieContainer == null)
                {
                    request.CookieContainer = new CookieContainer();
                }

                request.CookieContainer.Add(cookieCollection);
            }
        }

        void SetHeaders(WebHeaderCollection headers)
        {
            if (headers != null && headers.Count > 0)
            {
                foreach (string item in headers.AllKeys)
                {
                    request.Headers.Add(item, headers.Get(item));
                }
            }
        }

        void SetContentType(string contentType)
        {
            if (!String.IsNullOrEmpty(contentType))
            {
                request.ContentType = contentType;
            }
        }

        #endregion
    }
}
