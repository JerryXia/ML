using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using ML.Utils;
using System.Security.AccessControl;
using System.IO.MemoryMappedFiles;
using System.Threading;
using ML.Net.Http;

namespace TestForNET20
{
    class Program
    {
        static ManualResetEvent allDone = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            ThreadPool.SetMaxThreads(1024, 1024);

            switch (args[0])
            {
                case "w":
                    {
                        //while (true)
                        //{
                        Write(1, 2, DateTime.Now.ToString("HHmmssffffff"), args[1]);
                        //}
                    }
                    break;
                case "r":
                    {
                        while (true)
                        {
                            Read(args[1]);
                        }
                    }
                    break;
                default:
                    TestUri(RequestMethod.GET, "http://www.cnblogs.com/robots.txt", "");
                    TestUri(RequestMethod.GET, "http://www.guqiankun.com/robots.txt");
                    TestUri(RequestMethod.GET, "http://blog.guqiankun.com/robots.txt", "");

                    Console.ReadKey();
                    break;
            }
        }

        static void TestUri(RequestMethod method, string uri)
        {
            var requestSetting = new RequestSetting(method, uri);
            var requestState = new RequestState();
            requestState.Setting = requestSetting;
            requestState.Completed += new EventHandler<RequestCompeleteEventArgs>(RequestCompleted);

            var request = new Request();
            request.Send(requestState);
        }

        static void TestUri(RequestMethod method, string uri, string postData)
        {
            var requestSetting = new RequestSetting(method, uri);
            requestSetting.PostData = Encoding.UTF8.GetBytes(postData);
            var requestState = new RequestState();
            requestState.Setting = requestSetting;
            requestState.Completed += new EventHandler<RequestCompeleteEventArgs>(RequestCompleted);

            var request = new Request();
            request.Send(requestState);
        }

        static void RequestCompleted(object sender, RequestCompeleteEventArgs e)
        {
            var requestState = sender as RequestState;
            Console.WriteLine(e.HasError);
            Console.WriteLine(e.WebError);
            Console.WriteLine(e.ExtraError);

            Console.WriteLine(e.Resposne.StatusCode);
            Console.WriteLine(e.Resposne.StatusDescription);
            string result;
            Console.WriteLine(e.Resposne.TryGetStringResult(Encoding.UTF8, out result));
            Console.WriteLine(result);
        }

        static void Write(int p1, int p2, string p3, string filePath)
        {
            var shmem = new ProcessSharedMemory(filePath, "ShmemTest2");

            MyData data = new MyData()
            {
                myInt = p1,
                myPt = new Point()
                {
                    x = p2,
                    y = p3
                }
            };
            shmem.Write(data);
        }

        static void Read(string filePath)
        {
            var shmem = new ProcessSharedMemory(filePath, "ShmemTest2");

            MyData data = shmem.Read() as MyData;
            Console.WriteLine("{0}, {1}, {2}", data.myInt, data.myPt.x, data.myPt.y);
        }

    }

    [Serializable]
    public class Point
    {
        public int x;
        public string y;
    }

    [Serializable]
    public class MyData
    {
        public int myInt;
        public Point myPt;
    }
}
