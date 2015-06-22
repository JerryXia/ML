using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace ML.Utils
{
    /// <summary>
    /// 参考：http://www.codeproject.com/Articles/2635/High-Performance-Timer-in-C
    /// </summary>
    public class HiPerfTimer
    {
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long startTime, stopTime;
        private long freq;

        public HiPerfTimer()
        {
            startTime = 0;
            stopTime = 0;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                // high-performance counter not supported
                throw new Win32Exception();
            }
        }

        public void Start()
        {
            // lets do the waiting threads there work
            Thread.Sleep(0);

            QueryPerformanceCounter(out startTime);
        }

        public void Stop()
        {
            QueryPerformanceCounter(out stopTime);
        }

        /// <summary>
        /// 单位：秒
        /// </summary>
        public double Duration
        {
            get
            {
                return (double)(stopTime - startTime) / (double)freq;
            }
        }
    }
}
