using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ML.Security
{
    public class DESCryption
    {
        //默认密钥向量
        static byte[] DESIV = { 0x12, 0x34, 0x56, 0x90, 0xCD, 0xEF, 0xAB, 0x78 };



        public static string DESEncryptBase64(string encryptString, string key)
        {
            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.UTF8.GetBytes(key.PadRight(8, ' ').Substring(0, 8));
            des.IV = DESIV;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputData, 0, inputData.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string DesDecryptBase64(string decryptString, string key)
        {
            byte[] inputData = Convert.FromBase64String(decryptString);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.UTF8.GetBytes(key.PadRight(8, ' ').Substring(0, 8));
            des.IV = DESIV;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputData, 0, inputData.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        public static string DESEncrypt(string encryptString, string key)
        {
            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.UTF8.GetBytes(key.PadRight(8, ' ').Substring(0, 8));
            des.IV = DESIV;

            string result = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputData, 0, inputData.Length);
                    cs.FlushFinalBlock();
                    byte[] resArr = ms.ToArray();
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in resArr)
                    {
                        sb.AppendFormat("{0:X2}", b);
                    }
                    result = sb.ToString();
                }
            }
            return result;
        }

        public static string DesDecrypt(string decryptString, string key)
        {
            byte[] inputData = new byte[decryptString.Length / 2]; Convert.FromBase64String(decryptString);
            int arrayIndex = 0;
            foreach (byte b in inputData)
            {
                inputData[arrayIndex] = (byte)Convert.ToInt32(decryptString.Substring(arrayIndex * 2, 2), 16);
                arrayIndex = arrayIndex + 1;
            }

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = Encoding.UTF8.GetBytes(key.PadRight(8, ' ').Substring(0, 8));
            des.IV = DESIV;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputData, 0, inputData.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }


    }
}
