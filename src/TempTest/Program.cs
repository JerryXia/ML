using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.IO;

using ML;
using ML.Config;
using ML.Xml;
using ML.Utility;


namespace TempTest
{
    class Program
    {
        static void Main(string[] args)
        {

            //Console.WriteLine(Global.AppWebDir);
            //Console.WriteLine(Global.App_DataDir);

            //string xmlFileName = "CustomXmlConfig.xml";

            //CustomXmlConfig cusXmlConfig = new CustomXmlConfig(xmlFileName);

            //TestRunOption testRunOption1 = new TestRunOption { Name = "JerryXia", Age = 22 };
            //XmlSerialization.XmlSerializeToFile(testRunOption1, Path.Combine(Global.App_DataDir, xmlFileName), Encoding.UTF8);
            //IRunOptions testRunOption2 = cusXmlConfig.LoadRunOptions();
            //Console.WriteLine(testRunOption2.Age);
            //Console.WriteLine(testRunOption2.Name);
            //while(true)
            //{
                
            //}

            //CodeTimer.Time("Thread Sleep", 1, () => { Thread.Sleep(3000); });
            //CodeTimer.Time("Empty Method", 10000000, () => { });

            int iteration = 100 * 1000;

            string s = "";
            CodeTimer.Time("String Concat", iteration, () => { s += "a"; });

            StringBuilder sb = new StringBuilder();
            CodeTimer.Time("StringBuilder", iteration, () => { sb.Append("a"); });

            Console.ReadKey();
        }
    }

    //public class TestRunOption : IRunOptions
    //{
        //public string Name { get; set; }
       // public int Age { get; set; }
    //}
}
