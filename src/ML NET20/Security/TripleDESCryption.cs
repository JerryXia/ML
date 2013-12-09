using System;
using System.Text;
using System.Security.Cryptography;

namespace ML.Security
{
    public class TripleDESCryption
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public TripleDESCryption()
        {
            //
        }


        /// <summary>
        /// 使用给定密钥字符串加密string
        /// </summary>
        /// <param name="original">原始文字</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">字符编码方案</param>
        /// <returns>密文</returns>
        public static string TripleDESEncryptBase64(string original, string key)
        {
            byte[] buff = System.Text.Encoding.UTF8.GetBytes(original);
            byte[] kb = System.Text.Encoding.UTF8.GetBytes(key);
            return Convert.ToBase64String(TripleDESEncrypt(buff, kb));
        }
        /// <summary>
        /// 使用给定密钥字符串解密string
        /// </summary>
        /// <param name="original">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static string TripleDESDecryptBase64(string original, string key)
        {
            return TripleDESDecryptBase64(original, key, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 使用给定密钥字符串解密string,返回指定编码方式明文
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">字符编码方案</param>
        /// <returns>明文</returns>
        public static string TripleDESDecryptBase64(string encrypted, string key, Encoding encoding)
        {
            byte[] buff = Convert.FromBase64String(encrypted);
            byte[] kb = System.Text.Encoding.UTF8.GetBytes(key);
            return encoding.GetString(TripleDESDecrypt(buff, kb));
        }


        /// <summary>
        /// 使用给定密钥加密
        /// </summary>
        /// <param name="original">明文</param>
        /// <param name="key">密钥</param>
        /// <returns>密文</returns>
        public static byte[] TripleDESEncrypt(byte[] original, byte[] key)
        {
            TripleDESCryptoServiceProvider tripleDesProvider = new TripleDESCryptoServiceProvider();
            tripleDesProvider.Key = MD5Cryption.ComputeHash(key);
            tripleDesProvider.Mode = CipherMode.ECB;
            return tripleDesProvider.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
        }

        /// <summary>
        /// 使用给定密钥解密数据
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static byte[] TripleDESDecrypt(byte[] encrypted, byte[] key)
        {
            TripleDESCryptoServiceProvider tripleDesProvider = new TripleDESCryptoServiceProvider();
            tripleDesProvider.Key = MD5Cryption.ComputeHash(key);
            tripleDesProvider.Mode = CipherMode.ECB;
            return tripleDesProvider.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }


    }
}
