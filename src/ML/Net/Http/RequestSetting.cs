using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ML.Net.Http
{
    public class RequestSetting
    {
        #region General

        private RequestMethod _method = RequestMethod.GET;
        private string _url = null;
        private Version _protocolVersion;

        #endregion

        #region Request Headers

        private string _accept = null;
        private string _userAgent = null;

        private CookieCollection _cookieCollection = null;
        private string _cookie = null;
        private string _referer = null;

        private WebHeaderCollection _headers = null;

        #endregion

        #region 相关设置

        private string _cerPath = null;
        private X509CertificateCollection _clientCertificates;

        private string _proxyIp = null;
        private int _proxyPort;
        private string _proxyUsername = null;
        private string _proxyPwd = null;


        private int _timeout = 20000;
        private int _readWriteTimeout = 60000;
        private bool _allowAutoRedirect = false;

        private string _contentType = null;
        private byte[] _postData = null;

        #endregion

        public RequestSetting(RequestMethod method, string url)
        {
            _protocolVersion = HttpVersion.Version11;
            _method = method;
            _url = url;

            _accept = "text/html,application/xhtml+xml,*/*";
            _userAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/6.0)";
        }

        #region General

        public RequestMethod Method
        {
            get { return _method; }
            set { _method = value; }
        }

        public Version ProtocolVersion
        {
            get { return _protocolVersion; }
            set { _protocolVersion = value; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        #endregion

        #region Request Headers

        /// <summary>
        /// 请求标头值 默认为 text/html,application/xhtml+xml,*/*
        /// </summary>
        public string Accept
        {
            get { return _accept; }
            set { _accept = value; }
        }

        /// <summary>
        /// 客户端访问信息默认 Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/6.0)
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
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

        public WebHeaderCollection Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer
        {
            get { return _referer; }
            set { _referer = value; }
        }


        #endregion

        #region 相关设置

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
        /// 默认写入Post数据超时时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _readWriteTimeout; }
            set { _readWriteTimeout = value; }
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
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public bool AllowAutoRedirect
        {
            get { return _allowAutoRedirect; }
            set { _allowAutoRedirect = value; }
        }


        /// <summary>
        /// 请求类型默认 application/x-www-form-urlencoded
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostData
        {
            get { return _postData; }
            set { _postData = value; }
        }

        #endregion

    }
}
