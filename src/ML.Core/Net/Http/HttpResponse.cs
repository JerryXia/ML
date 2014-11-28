using System.Net;

namespace ML.Net.Http
{
    public class HttpResponse
    {
        private HttpStatusCode _statusCode;
        private string _statusDescription;
        private string _html;
        private byte[] _byteResult;
        private WebHeaderCollection _headers;
        private CookieCollection _cookies;

        /// <summary>
        /// 返回状态码,默认为OK
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription
        {
            get { return _statusDescription; }
            set { _statusDescription = value; }
        }

        /// <summary>
        /// 返回的String类型数据
        /// 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html
        {
            get { return _html; }
            set { _html = value; }
        }

        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ByteResult
        {
            get { return _byteResult; }
            set { _byteResult = value; }
        }

        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }


        public CookieCollection Cookies
        {
            get { return _cookies; }
            set { _cookies = value; }
        }




    }

}
