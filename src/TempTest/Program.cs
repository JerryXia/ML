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
using ML.Security;

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

            //int iteration = 100 * 1000;

            //string s = "";
            //CodeTimer.Time("String Concat", iteration, () => { s += "a"; });

            //StringBuilder sb = new StringBuilder();
            //CodeTimer.Time("StringBuilder", iteration, () => { sb.Append("a"); });

            string plainText = "1 - JerryXia - 24444444444444444444444444";
            string s1 = CommonCrypt.AESEncryption(plainText, "abcdefgh");
            string s2 = CommonCrypt.AESDecryption(s1, "abcdefgh");
            Console.WriteLine(s1 + "-----" + s2);

            string s3 = CommonCrypt.DESEncryption(plainText, "abc");
            string s4 = CommonCrypt.DESDecryption(s3, "abc");
            Console.WriteLine(s3 + "-----" + s4);

            Console.ReadKey();
        }
    }

    //public class TestRunOption : IRunOptions
    //{
        //public string Name { get; set; }
       // public int Age { get; set; }
    //}
}
