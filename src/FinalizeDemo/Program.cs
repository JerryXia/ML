using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalizeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TestOne();
            Log.Write("In Main..");
        }

        static void TestOne()
        {
            new Beijing();
            //Beijing bj = new Beijing();

            GC.Collect();
            //GC.WaitForPendingFinalizers();
            Log.Write("In TestOne..");
        }

    }
}
