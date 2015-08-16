using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using ML.Utils;
using System.Security.AccessControl;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace TestForNET20
{
    class Program
    {
        static void Main(string[] args)
        {
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
                    Console.WriteLine("None");
                    break;
            }
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
