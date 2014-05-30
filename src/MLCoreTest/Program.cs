using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ML.Framework.Utility;

namespace MLCoreTest
{
    class Program
    {
        static void Main(string[] args)
        {

            string originText = "1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz";
            string enString = CryptoHelper.AesEncrypt(originText, "1234567890");
            Console.WriteLine(enString);

            Console.WriteLine(CryptoHelper.AesDecrypt(enString, "1234567890"));

        }
    }
}
