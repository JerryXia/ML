using System;
using System.IO;
using System.Net;

namespace ML.Net.Http
{
    public class RequestState
    {
        #region Request相关

        public RequestSetting Setting;
        internal HttpWebRequest Request;

        #endregion


        internal const int BUFFER_SIZE = 1024;

        internal HttpWebResponse HttpWebResponse;
        internal Stream ResponseStream;
        internal string ContentType;

        internal byte[] BufferRead;
        internal MemoryStream ResponseMemoryStream;


        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            ResponseMemoryStream = new MemoryStream();
        }

        #region Response相关

        public event EventHandler<RequestCompeleteEventArgs> Completed;

        internal void RaiseRequestCompelete(object sender, RequestCompeleteEventArgs e)
        {
            if (Completed != null)
            {
                Completed(sender, e);
            }
        }

        #endregion

    }
}
