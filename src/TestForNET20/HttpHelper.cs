using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace TempTest
{
    public delegate void RequestCompeleteHandler(RequestCompeleteEventArgs e);

    public class RequestCompeleteEventArgs : EventArgs
    {
        public RequestCompeleteEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message { get; private set; }

        public List<KeyValuePair<string, string>> ResponseHeaders { get; set; }
        public string ResponseContent { get; set; }
        public string ContentType { get; set; }

    }

    public class RequestState
    {
        const int BUFFER_SIZE = 1024;

        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        public StringBuilder responseData;

        public event RequestCompeleteHandler RequestCompelete;
        public string contentType;
        public List<KeyValuePair<string, string>> responseHeaders;

        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
            responseData = new StringBuilder("");
            responseHeaders = new List<KeyValuePair<string, string>>();
        }

        public void RaiseRequestCompelete(string content)
        {
            if (RequestCompelete != null)
            {
                var e = new RequestCompeleteEventArgs("");
                e.ResponseHeaders = responseHeaders;
                e.ContentType = contentType;
                e.ResponseContent = content;
                RequestCompelete(e);
            }
        }
    }

    public class HttpHelper
    {
        //默认的编码
        private Encoding encoding = Encoding.UTF8;
        //Post数据编码
        private Encoding postencoding = Encoding.UTF8;

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;
        const int DefaultTimeout = 60 * 1000; // 1 minutes timeout

        public HttpHelper()
        {
            

        }

        //Accept:application/json, text/plain, */*
        //Accept-Encoding:gzip, deflate, sdch
        //Accept-Language:zh-CN,zh;q=0.8,zh-TW;q=0.6,en-US;q=0.4,en;q=0.2,en-GB;q=0.2
        //Connection:keep-alive
        //Host:10.0.3.200:9200
        //Origin:http://10.0.3.200
        //Referer:http://10.0.3.200/index.html
        //User-Agent:Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.155 Safari/537.36
        public void RequestUrl(string url, string postData, string method = "GET",
            string accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
            string contentType = "text/html",
            string referer = "",
            string userAgent = "robot",
            RequestCompeleteHandler reqEndHandler = null)
        {
            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.Method = method;
                myHttpWebRequest.Timeout = 60 * 1000;
                myHttpWebRequest.ReadWriteTimeout = 60 * 1000;
                myHttpWebRequest.Accept = accept;
                myHttpWebRequest.ContentType = contentType;
                if (!String.IsNullOrWhiteSpace(referer))
                {
                    myHttpWebRequest.Referer = referer;
                }
                myHttpWebRequest.UserAgent = userAgent;

                var requestState = new RequestState();
                requestState.request = myHttpWebRequest;
                requestState.requestData.Append(postData);
                if (reqEndHandler != null)
                {
                    requestState.RequestCompelete += new RequestCompeleteHandler(reqEndHandler);
                }

                if (method.Equals("post", StringComparison.OrdinalIgnoreCase) ||
                    method.Equals("put", StringComparison.OrdinalIgnoreCase)
                )
                {
                    IAsyncResult result = myHttpWebRequest
                        .BeginGetRequestStream(new AsyncCallback(RequestCallback), requestState);
                }
                else
                {
                    IAsyncResult responseResult = myHttpWebRequest
                        .BeginGetResponse(new AsyncCallback(RespCallback), requestState);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Abort the request if the timer fires.
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

        private void RequestCallback(IAsyncResult result)
        {
            try
            {
                ThreadPoolMessage("RequestStream Complete");

                var requestState = (RequestState)result.AsyncState;

                if (requestState.request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
                {
                    Stream postStream = requestState.request.EndGetRequestStream(result);
                    byte[] byteArray = postencoding.GetBytes(requestState.requestData.ToString());
                    postStream.Write(byteArray, 0, byteArray.Length);
                    postStream.Close();
                }

                //异步接收回传信息
                IAsyncResult responseResult = requestState.request
                    .BeginGetResponse(new AsyncCallback(RespCallback), requestState);

                // this line implements the timeout, if there is a timeout, the callback fires and the request becomes aborted
                //ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), requestState.request, DefaultTimeout, true);

                //The response came in the allowed time. The work processing will happen in the callback function.
                //allDone.WaitOne();
                // Release the HttpWebResponse resource.
                //requestState.response.Close();
            }
            catch
            {
            }
        }


        private void RespCallback(IAsyncResult result)
        {
            ThreadPoolMessage("BeginGetResponse Complete");
            try
            {
                RequestState requestState = (RequestState)result.AsyncState;
                requestState.response = (HttpWebResponse)requestState.request.EndGetResponse(result);

                Stream responseStream = requestState.response.GetResponseStream();
                requestState.streamResponse = responseStream;
                requestState.contentType = requestState.response.ContentType;

                for (int i = 0; i < requestState.response.Headers.Count; i++)
                {
                    string key = requestState.response.Headers.GetKey(i);
                    string value = requestState.response.Headers.Get(i);
                    requestState.responseHeaders.Add(new KeyValuePair<string, string>(key, value));
                }

                // Begin the Reading of the contents of the HTML page and print it to the console.
                IAsyncResult asynchronousInputRead = responseStream.BeginRead(requestState.BufferRead,
                    0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), requestState);
            }
            catch
            {
            }
        }

        private void ReadCallBack(IAsyncResult asyncResult)
        {
            ThreadPoolMessage("BeginRead Stream Complete");
            try
            {
                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);
                if (read > 0)
                {
                    myRequestState.responseData.Append(encoding.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                }
                else
                {
                    responseStream.Close();
                    myRequestState.RaiseRequestCompelete(myRequestState.responseData.ToString());
                }
            }
            catch (Exception e)
            {
            }
        }




        private void ThreadPoolMessage(string data)
        {
            /*
            int a, b;
            ThreadPool.GetAvailableThreads(out a, out b);
            string message = string.Format("{0}, CurrentThreadId is:{1}, WorkerThreads is:{2}, CompletionPortThreads is:{3}",
                  data, Thread.CurrentThread.ManagedThreadId, a.ToString(), b.ToString());

            Output(message);
            */
        }

        private void Output(string message)
        {
            //Debug.WriteLine(message);
            //Console.WriteLine(message);
        }

    }



}
