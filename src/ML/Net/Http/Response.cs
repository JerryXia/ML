using System;
using System.Net;
using System.Text;

namespace ML.Net.Http
{
    public class Response
    {
        private HttpStatusCode _statusCode;
        private string _statusDescription;
        private WebHeaderCollection _headers;
        private byte[] _result;

        private bool _hasConvertToString = false;
        private bool _convertToStringSuccess = false;
        private string _stringResult = null;

        public Response(HttpStatusCode statusCode, string statusDesc, WebHeaderCollection headers, byte[] result)
        {
            this._statusCode = statusCode;
            this._statusDescription = statusDesc;
            this._headers = headers;
            this._result = result;
        }

        /// <summary>
        /// 返回状态码,默认为OK
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription
        {
            get { return _statusDescription; }
        }

        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// 返回的Byte数组
        /// </summary>
        public byte[] ByteResult
        {
            get { return _result; }
        }

        /// <summary>
        /// 将返回的Byte尝试转化为字符串
        /// </summary>
        public bool TryGetStringResult(Encoding encoding, out string stringResult)
        {
            if (_hasConvertToString && _convertToStringSuccess)
            {
                stringResult = _stringResult;
                return _convertToStringSuccess;
            }
            else
            {
                try
                {
                    _stringResult = encoding.GetString(_result);
                    _hasConvertToString = true;
                    _convertToStringSuccess = true;
                }
                catch (Exception ex)
                {
                    _stringResult = string.Empty;
                    _hasConvertToString = true;
                    _convertToStringSuccess = false;
                }
                finally
                {
                    stringResult = _stringResult;
                }
                return _convertToStringSuccess;
            }
        }

    }

}
