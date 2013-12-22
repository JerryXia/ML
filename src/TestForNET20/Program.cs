using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Linq;

using ML;
using ML.Config;
using ML.Xml;
using ML.Utility;
using ML.Security;

namespace TestForNET20
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ML.NET20 Test");

            int[] numbers = { 5, 15, 7, 12 };

            var query =
              from n in numbers
              where n > 10
              orderby n
              select n * 10;

            Console.WriteLine(numbers.Where(p => p > 6).Aggregate((n1, n2) => n1+n2));

            Console.ReadKey();
        }
    }
}
