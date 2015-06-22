using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinalizeDemo
{
    using NLog;

    class Log
    {
        public static string logFilePath = "d:/1.txt";

        private static readonly Logger logger = NLog.LogManager.GetLogger("LogFile");

        public static void Write(string s)
        {
            Thread.Sleep(10);
            logger.Info(string.Format("{0}\tTotalMilliseconds:{1}\tTotalMemory:{2}", s, DateTime.Now.TimeOfDay.TotalMilliseconds, GC.GetTotalMemory(false)));
        }

    }
}
