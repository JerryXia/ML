using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ML.Security
{
    public class Md5CryptoService
    {
        private const int BUFFER_SIZE = 1048576;//8192;
        private static readonly MD5CryptoServiceProvider md5ServiceProvider = new MD5CryptoServiceProvider();

        public static byte[] ComputeHash(string originStr)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(originStr);
            return ComputeHash(byteData);
        }
        public static byte[] ComputeHash(byte[] originData)
        {
            return md5ServiceProvider.ComputeHash(originData);
        }

        public static string HashString(string originStr)
        {
            byte[] byteData = ComputeHash(originStr);
            return Md5String(byteData);
        }
        public static string HashString(byte[] originData)
        {
            byte[] byteData = ComputeHash(originData);
            return Md5String(byteData);
        }

        static string Md5String(byte[] inputData)
        {
            StringBuilder sb = new StringBuilder(inputData.Length * 2);
            foreach (byte b in inputData)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
        static string Md5String(Stream Content)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] hash = md5.ComputeHash(Content);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
        /// <summary>
        /// 计算文件的MD5的hash值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileHashString(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE))
            {
                string result = Md5String(stream);
                return result;
            }
        }

    }
}
