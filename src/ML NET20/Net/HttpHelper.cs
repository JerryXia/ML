using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace ML.Net
{
    /// <example>
    ///  HttpHelper http = new HttpHelper();
    ///  HttpItem item = new HttpItem()
    ///  {
    ///   URL = "http://www.cckan.net",//URL     必需项
    ///   Encoding = "gbk",//编码格式（utf-8,gb2312,gbk）     可选项 默认类会自动识别
    ///   Method = "get",//URL     可选项 默认为Get
    ///   //Timeout = 100000,//连接超时时间     可选项默认为100000
    ///   //ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000
    ///   //IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写
    ///   Cookie = "",//字符串Cookie     可选项
    ///   // UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值
    ///   // Accept = "text/html, application/xhtml+xml, */*",//    可选项有默认值
    ///   // ContentType = "text/html",//返回类型    可选项有默认值
    ///   Referer = "http://www.cckan.net",//来源URL     可选项
    ///   //Allowautoredirect = true,//是否根据３０１跳转     可选项
    ///   //CerPath = "d:\\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数
    ///   //Connectionlimit = 1024,//最大连接数     可选项 默认为1024
    ///   //Postdata = "username=sufei&pwd=cckan.net",//Post数据     可选项GET时不需要写
    ///   //ProxyIp = "192.168.1.105",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数
    ///   //ProxyPwd = "123456",//代理服务器密码     可选项
    ///   // ProxyUserName = "administrator",//代理服务器账户名     可选项
    ///  };
    ///  //得到HTML代码
    ///  string html = http.GetHtml(item);
    ///
    ///  //取出返回的Cookie
    ///  string cookie = item.Cookie;
    ///  //取出返回的Request
    ///  HttpWebRequest request = item.Request;
    ///  //取出返回的Response
    ///  HttpWebResponse response = item.Response;
    ///  //取出返回的Reader
    ///  StreamReader reader = item.Reader;
    ///  //取出返回的Headers
    ///  WebHeaderCollection header = response.Headers;
    /// </example>

    /// <summary>
    /// Http连接操作帮助类
    /// </summary>
    public class HttpHelper
    {
        #region 预定义方法或者变更

        //默认的编码
        private Encoding encoding = Encoding.Default;
        //Post数据编码
        private Encoding postencoding = Encoding.Default;
        //HttpWebRequest对象用来发起请求
        private HttpWebRequest request = null;
        //获取影响流的数据对象
        private HttpWebResponse response = null;
        /// <summary>
        /// 根据相传入的数据，得到相应页面数据
        /// </summary>
        /// <param name="objhttpitem">参数类对象</param>
        /// <returns>返回HttpResult类型</returns>
        public HttpResult GetHtml(HttpItem objhttpitem)
        {
            //返回参数
            HttpResult result = new HttpResult();
            try
            {
                //准备参数
                SetRequest(objhttpitem);
            }
            catch (Exception ex)
            {
                result = new HttpResult();
                result.Cookie = string.Empty;
                result.Header = null;
                result.Html = ex.Message;
                result.StatusDescription = "配置参数时出错：" + ex.Message;
                return result;
            }
            try
            {
                #region 得到请求的response
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    result.StatusCode = response.StatusCode;
                    result.StatusDescription = response.StatusDescription;
                    result.Header = response.Headers;
                    if (response.Cookies != null) result.CookieCollection = response.Cookies;
                    if (response.Headers["set-cookie"] != null) result.Cookie = response.Headers["set-cookie"];
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
                    byte[] RawResponse = _stream.ToArray();
                    _stream.Close();
                    //是否返回Byte类型数据
                    if (objhttpitem.ResultType == ResultType.Byte) result.ResultByte = RawResponse;
                    //从这里开始我们要无视编码了
                    if (encoding == null)
                    {
                        Match meta = Regex.Match(Encoding.Default.GetString(RawResponse), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                        string charter = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToLower() : string.Empty;
                        if (charter.Length > 2)
                            encoding = Encoding.GetEncoding(charter.Trim().Replace("\"", "").Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk"));
                        else
                        {
                            if (string.IsNullOrEmpty(response.CharacterSet)) encoding = Encoding.UTF8;
                            else encoding = Encoding.GetEncoding(response.CharacterSet);
                        }
                    }
                    //得到返回的HTML
                    result.Html = encoding.GetString(RawResponse);
                }
                #endregion
            }
            catch (WebException ex)
            {
                //这里是在发生异常时返回的错误信息
                response = (HttpWebResponse)ex.Response;
                result.Html = ex.Message;
                if (response != null)
                {
                    result.StatusCode = response.StatusCode;
                    result.StatusDescription = response.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                result.Html = ex.Message;
            }
            if (objhttpitem.IsToLower) result.Html = result.Html.ToLower();
            return result;
        }
        /// <summary>
        /// 4.0以下.net版本取数据使用
        /// </summary>
        /// <param name="streamResponse">流</param>
        private static MemoryStream GetMemoryStream(Stream streamResponse)
        {
            MemoryStream _stream = new MemoryStream();
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = streamResponse.Read(buffer, 0, Length);
            while (bytesRead > 0)
            {
                _stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, Length);
            }
            return _stream;
        }
        /// <summary>
        /// 为请求准备参数
        /// </summary>
        ///<param name="objhttpItem">参数列表</param>
        private void SetRequest(HttpItem objhttpItem)
        {
            // 验证证书
            SetCer(objhttpItem);
            //设置Header参数
            if (objhttpItem.Header != null && objhttpItem.Header.Count > 0) foreach (string item in objhttpItem.Header.AllKeys)
                {
                    request.Headers.Add(item, objhttpItem.Header[item]);
                }
            // 设置代理
            SetProxy(objhttpItem);
            if (objhttpItem.ProtocolVersion != null) request.ProtocolVersion = objhttpItem.ProtocolVersion;
            request.ServicePoint.Expect100Continue = objhttpItem.Expect100Continue;
            //请求方式Get或者Post
            request.Method = objhttpItem.Method;
            request.Timeout = objhttpItem.Timeout;
            request.ReadWriteTimeout = objhttpItem.ReadWriteTimeout;
            //Accept
            request.Accept = objhttpItem.Accept;
            //ContentType返回类型
            request.ContentType = objhttpItem.ContentType;
            //UserAgent客户端的访问类型，包括浏览器版本和操作系统信息
            request.UserAgent = objhttpItem.UserAgent;
            // 编码
            encoding = objhttpItem.Encoding;
            //设置Cookie
            SetCookie(objhttpItem);
            //来源地址
            request.Referer = objhttpItem.Referer;
            //是否执行跳转功能
            request.AllowAutoRedirect = objhttpItem.Allowautoredirect;
            //设置Post数据
            SetPostData(objhttpItem);
            //设置最大连接
            if (objhttpItem.Connectionlimit > 0) request.ServicePoint.ConnectionLimit = objhttpItem.Connectionlimit;
        }
        /// <summary>
        /// 设置证书
        /// </summary>
        /// <param name="objhttpItem"></param>
        private void SetCer(HttpItem objhttpItem)
        {
            if (!string.IsNullOrEmpty(objhttpItem.CerPath))
            {
                //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //初始化对像，并设置请求的URL地址
                request = (HttpWebRequest)WebRequest.Create(objhttpItem.URL);
                SetCerList(objhttpItem);
                //将证书添加到请求里
                request.ClientCertificates.Add(new X509Certificate(objhttpItem.CerPath));
            }
            else
            {
                //初始化对像，并设置请求的URL地址
                request = (HttpWebRequest)WebRequest.Create(objhttpItem.URL);
                SetCerList(objhttpItem);
            }
        }
        /// <summary>
        /// 设置多个证书
        /// </summary>
        /// <param name="objhttpItem"></param>
        private void SetCerList(HttpItem objhttpItem)
        {
            if (objhttpItem.ClentCertificates != null && objhttpItem.ClentCertificates.Count > 0)
            {
                foreach (X509Certificate item in objhttpItem.ClentCertificates)
                {
                    request.ClientCertificates.Add(item);
                }
            }
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="objhttpItem">Http参数</param>
        private void SetCookie(HttpItem objhttpItem)
        {
            if (!string.IsNullOrEmpty(objhttpItem.Cookie))
                //Cookie
                request.Headers[HttpRequestHeader.Cookie] = objhttpItem.Cookie;
            //设置Cookie
            if (objhttpItem.CookieCollection != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(objhttpItem.CookieCollection);
            }
        }
        /// <summary>
        /// 设置Post数据
        /// </summary>
        /// <param name="objhttpItem">Http参数</param>
        private void SetPostData(HttpItem objhttpItem)
        {
            //验证在得到结果时是否有传入数据
            if (request.Method.Trim().ToLower().Contains("post"))
            {
                if (objhttpItem.PostEncoding != null)
                {
                    postencoding = objhttpItem.PostEncoding;
                }
                byte[] buffer = null;
                //写入Byte类型
                if (objhttpItem.PostDataType == PostDataType.Byte && objhttpItem.PostdataByte != null && objhttpItem.PostdataByte.Length > 0)
                {
                    //验证在得到结果时是否有传入数据
                    buffer = objhttpItem.PostdataByte;
                }//写入文件
                else if (objhttpItem.PostDataType == PostDataType.FilePath && !string.IsNullOrEmpty(objhttpItem.Postdata))
                {
                    StreamReader r = new StreamReader(objhttpItem.Postdata, postencoding);
                    buffer = postencoding.GetBytes(r.ReadToEnd());
                    r.Close();
                } //写入字符串
                else if (!string.IsNullOrEmpty(objhttpItem.Postdata))
                {
                    buffer = postencoding.GetBytes(objhttpItem.Postdata);
                }
                if (buffer != null)
                {
                    request.ContentLength = buffer.Length;
                    request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
            }
        }
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="objhttpItem">参数对象</param>
        private void SetProxy(HttpItem objhttpItem)
        {
            if (!string.IsNullOrEmpty(objhttpItem.ProxyIp))
            {
                //设置代理服务器
                if (objhttpItem.ProxyIp.Contains(":"))
                {
                    string[] plist = objhttpItem.ProxyIp.Split(':');
                    WebProxy myProxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()));
                    //建议连接
                    myProxy.Credentials = new NetworkCredential(objhttpItem.ProxyUserName, objhttpItem.ProxyPwd);
                    //给当前请求对象
                    request.Proxy = myProxy;
                }
                else
                {
                    WebProxy myProxy = new WebProxy(objhttpItem.ProxyIp, false);
                    //建议连接
                    myProxy.Credentials = new NetworkCredential(objhttpItem.ProxyUserName, objhttpItem.ProxyPwd);
                    //给当前请求对象
                    request.Proxy = myProxy;
                }
                //设置安全凭证
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
        }
        /// <summary>
        /// 回调验证证书问题
        /// </summary>
        /// <param name="sender">流对象</param>
        /// <param name="certificate">证书</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        #endregion
    }
    /// <summary>
    /// Http请求参考类
    /// </summary>
    public class HttpItem
    {
        string _URL = string.Empty;
        /// <summary>
        /// 请求URL必须填写
        /// </summary>
        public string URL
        {
            get { return _URL; }
            set { _URL = value; }
        }
        string _Method = "GET";
        /// <summary>
        /// 请求方式默认为GET方式,当为POST方式时必须设置Postdata的值
        /// </summary>
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        int _Timeout = 20000;
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        int _ReadWriteTimeout = 30000;
        /// <summary>
        /// 默认写入Post数据超时时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _ReadWriteTimeout; }
            set { _ReadWriteTimeout = value; }
        }
        string _Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        string _ContentType = "text/html";
        /// <summary>
        /// 请求返回类型默认 text/html
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        string _UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        Encoding _Encoding = null;
        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding
        {
            get { return _Encoding; }
            set { _Encoding = value; }
        }
        private PostDataType _PostDataType = PostDataType.String;
        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType
        {
            get { return _PostDataType; }
            set { _PostDataType = value; }
        }
        string _Postdata = string.Empty;
        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata
        {
            get { return _Postdata; }
            set { _Postdata = value; }
        }
        private byte[] _PostdataByte = null;
        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte
        {
            get { return _PostdataByte; }
            set { _PostdataByte = value; }
        }
        CookieCollection cookiecollection = null;
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return cookiecollection; }
            set { cookiecollection = value; }
        }
        string _Cookie = string.Empty;
        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _Cookie; }
            set { _Cookie = value; }
        }
        string _Referer = string.Empty;
        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer
        {
            get { return _Referer; }
            set { _Referer = value; }
        }
        string _CerPath = string.Empty;
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath
        {
            get { return _CerPath; }
            set { _CerPath = value; }
        }
        private Boolean isToLower = false;
        /// <summary>
        /// 是否设置为全文小写，默认为不转化
        /// </summary>
        public Boolean IsToLower
        {
            get { return isToLower; }
            set { isToLower = value; }
        }
        private Boolean allowautoredirect = false;
        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }
        private int connectionlimit = 1024;
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit
        {
            get { return connectionlimit; }
            set { connectionlimit = value; }
        }
        private string proxyusername = string.Empty;
        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName
        {
            get { return proxyusername; }
            set { proxyusername = value; }
        }
        private string proxypwd = string.Empty;
        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd
        {
            get { return proxypwd; }
            set { proxypwd = value; }
        }
        private string proxyip = string.Empty;
        /// <summary>
        /// 代理 服务IP
        /// </summary>
        public string ProxyIp
        {
            get { return proxyip; }
            set { proxyip = value; }
        }
        private ResultType resulttype = ResultType.String;
        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType
        {
            get { return resulttype; }
            set { resulttype = value; }
        }
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }

        private Version _ProtocolVersion;

        /// <summary>
        /// 获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
        /// </summary>
        public Version ProtocolVersion
        {
            get { return _ProtocolVersion; }
            set { _ProtocolVersion = value; }
        }
        private Boolean _expect100continue = true;
        /// <summary>
        /// 获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        public Boolean Expect100Continue
        {
            get { return _expect100continue; }
            set { _expect100continue = value; }
        }
        private X509CertificateCollection _ClentCertificates;
        /// <summary>
        /// 设置509证书集合
        /// </summary>
        public X509CertificateCollection ClentCertificates
        {
            get { return _ClentCertificates; }
            set { _ClentCertificates = value; }
        }
        private Encoding _PostEncoding;
        /// <summary>
        /// 设置或获取Post参数编码,默认的为Default编码
        /// </summary>
        public Encoding PostEncoding
        {
            get { return _PostEncoding; }
            set { _PostEncoding = value; }
        }
    }
    /// <summary>
    /// Http返回参数类
    /// </summary>
    public class HttpResult
    {
        private string _Cookie;
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _Cookie; }
            set { _Cookie = value; }
        }

        private CookieCollection _CookieCollection;
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return _CookieCollection; }
            set { _CookieCollection = value; }
        }
        private string _Html;
        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html
        {
            get { return _Html; }
            set { _Html = value; }
        }
        private byte[] _ResultByte;
        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte
        {
            get { return _ResultByte; }
            set { _ResultByte = value; }
        }

        private WebHeaderCollection _Header;
        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return _Header; }
            set { _Header = value; }
        }

        private string _StatusDescription;
        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription
        {
            get { return _StatusDescription; }
            set { _StatusDescription = value; }
        }
        private HttpStatusCode _StatusCode;
        /// <summary>
        /// 返回状态码,默认为OK
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _StatusCode; }
            set { _StatusCode = value; }
        }
    }
    /// <summary>
    /// 返回类型
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 表示只返回字符串 只有Html有数据
        /// </summary>
        String,
        /// <summary>
        /// 表示返回字符串和字节流 ResultByte和Html都有数据返回
        /// </summary>
        Byte
    }
    /// <summary>
    /// Post的数据格式默认为string
    /// </summary>
    public enum PostDataType
    {
        /// <summary>
        /// 字符串类型，这时编码Encoding可不设置
        /// </summary>
        String,
        /// <summary>
        /// Byte类型，需要设置PostdataByte参数的值编码Encoding可设置为空
        /// </summary>
        Byte,
        /// <summary>
        /// 传文件，Postdata必须设置为文件的绝对路径，必须设置Encoding的值
        /// </summary>
        FilePath
    }
}