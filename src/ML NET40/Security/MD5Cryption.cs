using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ML.Security
{
    public class MD5Cryption
    {
        const int BufferSize = 1048576;//8192;
        static readonly MD5CryptoServiceProvider md5ServiceProvider = new MD5CryptoServiceProvider();


        public static byte[] ComputeHash(string originStr)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(originStr);
            return ComputeHash(byteData);
        }
        public static byte[] ComputeHash(byte[] originData)
        {
            return md5ServiceProvider.ComputeHash(originData);
        }


        /// <summary>
        /// MD5函数 --> 转换为base64字符串
        /// </summary>
        /// <param name="originStr">初始字符串</param>
        /// <returns></returns>
        public static string MD5Base64String(string originStr)
        {
            byte[] byteData = ComputeHash(originStr);
            return MD5Base64(byteData);
        }
        /// <summary>
        /// MD5函数 --> 转换为base64字符串
        /// </summary>
        /// <param name="originData">初始二进制流</param>
        /// <returns></returns>
        public static string MD5Base64String(byte[] originData)
        {
            byte[] byteData = ComputeHash(originData);
            return MD5Base64(byteData);
        }
        /// <summary>
        /// 基础方法 MD5函数 --> 转换为base64字符串
        /// </summary>
        /// <param name="byteData">ComputeHash计算后的二进制流</param>
        /// <returns></returns>
        static string MD5Base64(byte[] byteData)
        {
            return Convert.ToBase64String(byteData);
        }


        public static string MD5String(string originStr)
        {
            byte[] byteData = ComputeHash(originStr);
            return MD5(byteData);
        }
        public static string MD5String(byte[] originData)
        {
            byte[] byteData = ComputeHash(originData);
            return MD5(byteData);
        }
        static string MD5(byte[] byteData)
        {
            StringBuilder sb = new StringBuilder(byteData.Length * 2);
            foreach (byte b in byteData)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }



        /// <summary>
        /// 计算文件的MD5的hash值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileMD5String(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize))
            {
                string result = HashMD5Content(stream);
                stream.Close();
                return result;
            }
        }
        static string HashMD5Content(Stream Content)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] hash = md5.ComputeHash(Content);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }


    }
}
