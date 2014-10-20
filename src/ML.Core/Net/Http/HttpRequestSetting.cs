using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ML.Net
{
    public class HttpRequestSetting
    {
        #region private fields

        string _url = null;

        HttpRequestMethod _method = HttpRequestMethod.GET;

        int _timeout = 20000;
        int _readWriteTimeout = 30000;

        string _accept = null;
        string _contentType = null;
        string _userAgent = null;

        Encoding _encoding = null;
        HttpRequestDataMode _requestDataMode = HttpRequestDataMode.String;
        string _postStringData = null;
        byte[] _postByteData = null;

        CookieCollection _cookieCollection = null;
        string _cookie = null;

        string _referer = null;

        WebHeaderCollection _headers = null;


        string _cerPath = null;
        private bool _allowAutoRedirect = false;
        private int _connectionLimit = 1024;

        private string _proxyUsername = null;
        private string _proxyPwd = null;
        private string _proxyIp = null;
        private int _proxyPort;

        private Version _protocolVersion;
        private bool _expect100continue = true;
        private X509CertificateCollection _clientCertificates;

        private HttpResponseDataType _resultDataType;

        #endregion


        public HttpRequestSetting(string url, HttpRequestMethod method, Encoding encoding)
        {
            _url = url;
            _method = method;
            _accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            _contentType = "text/html";
            _userAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            _encoding = encoding;
            _protocolVersion = HttpVersion.Version11;
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public HttpRequestMethod Method
        {
            get { return _method; }
            set { _method = value; }
        }

        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// 默认写入Post数据超时时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _readWriteTimeout; }
            set { _readWriteTimeout = value; }
        }

        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept
        {
            get { return _accept; }
            set { _accept = value; }
        }

        /// <summary>
        /// 请求返回类型默认 text/html
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        /// <summary>
        /// Post的数据类型
        /// </summary>
        public HttpRequestDataMode DataMode
        {
            get { return _requestDataMode; }
            set { _requestDataMode = value; }
        }

        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string PostStringData
        {
            get { return _postStringData; }
            set { _postStringData = value; }
        }

        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostByteData
        {
            get { return _postByteData; }
            set { _postByteData = value; }
        }

        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return _cookieCollection; }
            set { _cookieCollection = value; }
        }

        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _cookie; }
            set { _cookie = value; }
        }

        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer
        {
            get { return _referer; }
            set { _referer = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public bool AllowAutoRedirect
        {
            get { return _allowAutoRedirect; }
            set { _allowAutoRedirect = value; }
        }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int ConnectionLimit
        {
            get { return _connectionLimit; }
            set { _connectionLimit = value; }
        }

        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName
        {
            get { return _proxyUsername; }
            set { _proxyUsername = value; }
        }

        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd
        {
            get { return _proxyPwd; }
            set { _proxyPwd = value; }
        }

        /// <summary>
        /// 代理 服务IP
        /// </summary>
        public string ProxyIp
        {
            get { return _proxyIp; }
            set { _proxyIp = value; }
        }

        public int ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }

        /// <summary>
        /// 获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
        /// </summary>
        public Version ProtocolVersion
        {
            get { return _protocolVersion; }
            set { _protocolVersion = value; }
        }

        /// <summary>
        /// 获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        public Boolean Expect100Continue
        {
            get { return _expect100continue; }
            set { _expect100continue = value; }
        }

        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath
        {
            get { return _cerPath; }
            set { _cerPath = value; }
        }

        /// <summary>
        /// 设置509证书集合
        /// </summary>
        public X509CertificateCollection ClientCertificates
        {
            get { return _clientCertificates; }
            set { _clientCertificates = value; }
        }


        public HttpResponseDataType ResultDataType
        {
            get { return _resultDataType; }
            set { _resultDataType = value; }
        }

    }
}
