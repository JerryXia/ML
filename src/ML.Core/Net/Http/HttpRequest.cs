using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ML.Net
{
    public class HttpRequest
    {
        #region Fields

        //默认的编码
        private readonly Encoding DEFAULT_ENCODING = Encoding.UTF8;


        private Encoding encoding = null;

        //HttpWebRequest对象用来发起请求
        private HttpWebRequest request = null;

        //获取影响流的数据对象
        private HttpWebResponse response = null;

        #endregion


        #region Public Func

        public HttpResponse GetResponse(HttpRequestSetting setting)
        {
            HttpResponse result = new HttpResponse();
            try
            {
                SetHttpRequest(setting);
            }
            catch (Exception ex)
            {
                result.Html = ex.ToString();
                return result;
            }

            try
            {
                // 获取请求的响应
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    result.StatusCode = response.StatusCode;
                    result.StatusDescription = response.StatusDescription;
                    result.Headers = response.Headers;

                    if (response.Cookies != null)
                        result.Cookies = response.Cookies;

                    //if (response.Headers["set-cookie"] != null)
                    //    result.Cookie = response.Headers["set-cookie"];

                    MemoryStream _stream = new MemoryStream();
                    //GZIIP处理
                    if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //开始读取流并设置编码方式
                        //new GZipStream(response.GetResponseStream(), CompressionMode.Decompress).CopyTo(_stream, 10240);
                        //.net4.0以下写法
                        _stream = GetMemoryStream(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
                    }
                    else
                    {
                        //开始读取流并设置编码方式
                        //response.GetResponseStream().CopyTo(_stream, 10240);
                        //.net4.0以下写法
                        _stream = GetMemoryStream(response.GetResponseStream());
                    }
                    //获取Byte
                    byte[] rawResponse = _stream.ToArray();
                    _stream.Close();

                    //是否返回Byte类型数据
                    if (setting.ResultDataType == HttpResponseDataType.Byte)
                    {
                        result.ByteResult = rawResponse;
                    }

                    //if (encoding == null)
                    //{
                        //Match meta = Regex.Match(Encoding.Default.GetString(rawResponse), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                        //string charter = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToLower() : string.Empty;
                        //if (charter.Length > 2)
                        //    encoding = Encoding.GetEncoding(charter.Trim().Replace("\"", "").Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk"));
                        //else
                        //{
                        //    if (string.IsNullOrEmpty(response.CharacterSet)) encoding = Encoding.UTF8;
                        //    else encoding = Encoding.GetEncoding(response.CharacterSet);
                        //}
                    //}
                    result.Html = encoding.GetString(rawResponse);
                }
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                result.Html = ex.ToString();
                if (response != null)
                {
                    result.StatusCode = response.StatusCode;
                    result.StatusDescription = response.StatusDescription;
                    result.Headers = response.Headers;
                }
            }
            catch (Exception ex)
            {
                result.Html = ex.ToString();
            }

            return result;
        }

        #endregion


        #region Private Func

        void SetHttpRequest(HttpRequestSetting setting)
        {
            InitHttpRequest(setting.Url);

            // 验证证书
            SetCerList(setting.ClientCertificates);
            SetCer(setting.CerPath);

            // 设置代理
            SetProxy(setting.ProxyIp, setting.ProxyPort, setting.ProxyUserName, setting.ProxyPwd);

            request.ProtocolVersion = setting.ProtocolVersion;
            request.ServicePoint.Expect100Continue = setting.Expect100Continue;
            request.ServicePoint.ConnectionLimit = setting.ConnectionLimit;
            request.Method = SetMethod(setting.Method);
            request.Timeout = setting.Timeout;
            request.ReadWriteTimeout = setting.ReadWriteTimeout;
            request.Accept = setting.Accept;
            request.ContentType = setting.ContentType;
            request.UserAgent = setting.UserAgent;
            request.Referer = setting.Referer;
            request.AllowAutoRedirect = setting.AllowAutoRedirect;

            encoding = setting.Encoding == null ? DEFAULT_ENCODING : setting.Encoding;

            //设置Cookie
            SetCookie(setting.Cookie, setting.CookieCollection);

            //设置自定义头
            SetHeaders(setting.Headers);

            //设置提交数据
            SetPostData(setting.Method, setting.Encoding, setting.DataMode, setting.PostStringData, setting.PostByteData);
        }


        void InitHttpRequest(string url)
        {
            //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            request = (HttpWebRequest)WebRequest.Create(url);
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
            if (!string.IsNullOrEmpty(cerPath))
            {
                //将证书添加到请求里
                X509Certificate x509 = new X509Certificate(cerPath);
                request.ClientCertificates.Add(x509);
            }
        }

        void SetProxy(string ip, int port, string userName, string passWord)
        {
            if (!string.IsNullOrEmpty(ip))
            {
                WebProxy myProxy = new WebProxy(ip, port);
                //建议连接
                myProxy.Credentials = new NetworkCredential(userName, passWord);
                //给当前请求对象
                request.Proxy = myProxy;
                //设置安全凭证
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
        }

        string SetMethod(HttpRequestMethod method)
        {
            string returnMethod = null;
            switch (method)
            {
                case HttpRequestMethod.POST:
                    returnMethod = "POST";
                    break;
                case HttpRequestMethod.PUT:
                    returnMethod = "PUT";
                    break;
                case HttpRequestMethod.PATCH:
                    returnMethod = "PATCH";
                    break;
                case HttpRequestMethod.DELETE:
                    returnMethod = "DELETE";
                    break;
                case HttpRequestMethod.COPY:
                    returnMethod = "COPY";
                    break;
                case HttpRequestMethod.HEAD:
                    returnMethod = "HEAD";
                    break;
                case HttpRequestMethod.OPTIONS:
                    returnMethod = "OPTIONS";
                    break;
                case HttpRequestMethod.LINK:
                    returnMethod = "LINK";
                    break;
                case HttpRequestMethod.UNLINK:
                    returnMethod = "UNLINK";
                    break;
                case HttpRequestMethod.PURGE:
                    returnMethod = "PURGE";
                    break;
                case HttpRequestMethod.GET:
                default:
                    returnMethod = "GET";
                    break;
            }
            return returnMethod;
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

        void SetPostData(HttpRequestMethod method, Encoding encoding, HttpRequestDataMode dataMode, string stringData, byte[] byteData)
        {
            if (method == HttpRequestMethod.POST)
            {
                if (encoding == null)
                {
                    encoding = DEFAULT_ENCODING;
                }

                byte[] buffer = null;
                if (dataMode == HttpRequestDataMode.Byte && byteData != null && byteData.Length > 0)
                {
                    buffer = byteData;
                }
                //else if (objhttpItem.PostDataType == PostDataType.FilePath && !string.IsNullOrEmpty(objhttpItem.Postdata))
                //{
                //    StreamReader r = new StreamReader(objhttpItem.Postdata, postencoding);
                //    buffer = postencoding.GetBytes(r.ReadToEnd());
                //    r.Close();
                //}
                else if (!string.IsNullOrEmpty(stringData))
                {
                    buffer = encoding.GetBytes(stringData);
                }
                else { }

                if (buffer != null)
                {
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
            }
        }


        static MemoryStream GetMemoryStream(Stream streamResponse)
        {
            MemoryStream _stream = new MemoryStream();
            int Length = 256;
            byte[] buffer = new byte[Length];
            int bytesRead = streamResponse.Read(buffer, 0, Length);
            while (bytesRead > 0)
            {
                _stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, Length);
            }
            return _stream;
        }

        #endregion
    }
}
