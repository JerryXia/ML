using System;
using System.Text;
using System.Security.Cryptography;

namespace ML.Security
{
    public class SHA256Cryption
    {

        public static string SHA256Base64(string originStr)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(originStr);
            SHA256Managed sha256 = new SHA256Managed();
            byte[] result = sha256.ComputeHash(SHA256Data);
            //返回长度为44字节的字符串
            return Convert.ToBase64String(result);
        }

        public static string SHA256(string originStr)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(originStr);
            SHA256Managed sha256 = new SHA256Managed();
            byte[] result = sha256.ComputeHash(SHA256Data);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in result)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }
    }
}
