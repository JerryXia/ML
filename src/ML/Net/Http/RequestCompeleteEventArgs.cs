using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ML.Net.Http
{
    public class RequestCompeleteEventArgs : EventArgs
    {
        private bool _hasError = false;
        private WebException _webError = null;
        private Exception _extraError = null;

        private Response _resposne;

        public RequestCompeleteEventArgs(bool hasError)
        {
            _hasError = hasError;
        }

        public RequestCompeleteEventArgs(Response resonse)
        {
            _resposne = resonse;
        }


        public Response Resposne
        {
            get { return _resposne; }
            set { this._resposne = value; }
        }


        public bool HasError
        {
            get { return _hasError; }
            set { this._hasError = value; }
        }

        public WebException WebError
        {
            get { return _webError; }
            set { this._webError = value; }
        }

        public Exception ExtraError
        {
            get { return _extraError; }
            set { this._extraError = value; }
        }

    }
}
