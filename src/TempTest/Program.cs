using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using ML;
using ML.Config;
using ML.Xml;

namespace TempTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(Global.AppWebDir);
            Console.WriteLine(Global.App_DataDir);

            string xmlFileName = "CustomXmlConfig.xml";

            CustomXmlConfig cusXmlConfig = new CustomXmlConfig(xmlFileName);

            //TestRunOption testRunOption1 = new TestRunOption { Name = "JerryXia", Age = 22 };
            //XmlSerialization.XmlSerializeToFile(testRunOption1, Path.Combine(Global.App_DataDir, xmlFileName), Encoding.UTF8);
            //IRunOptions testRunOption2 = cusXmlConfig.LoadRunOptions();
            //Console.WriteLine(testRunOption2.Age);
            //Console.WriteLine(testRunOption2.Name);
            //while(true)
            //{
                
            //}

            Console.ReadKey();
        }
    }

    public class TestRunOption : IRunOptions
    {
        //public string Name { get; set; }
       // public int Age { get; set; }
    }
}
