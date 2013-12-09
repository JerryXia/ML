using System;
using System.Text;
using System.Security.Cryptography;

namespace ML.Security
{
    public class AESCryption
    {
        //默认密钥向量
        static byte[] AESIV = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

        public AESCryption()
        {
            //
        }

        public static string AESEncryptBase64(string encryptString, string key)
        {
            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);

            RijndaelManaged rijndaelManager = new RijndaelManaged();
            rijndaelManager.Key = Encoding.UTF8.GetBytes(key.PadRight(32, ' ').Substring(0, 32));
            rijndaelManager.IV = AESIV;
            rijndaelManager.Mode = CipherMode.CBC;
            rijndaelManager.Padding = PaddingMode.PKCS7;

            ICryptoTransform rijndaelEncryptor = rijndaelManager.CreateEncryptor();

            byte[] encryptedData = rijndaelEncryptor.TransformFinalBlock(inputData, 0, inputData.Length);

            rijndaelEncryptor.Dispose();
            rijndaelManager.Clear();

            return Convert.ToBase64String(encryptedData);
        }

        public static string AESDecryptBase64(string decryptString, string key)
        {
            byte[] inputData = Convert.FromBase64String(decryptString);

            RijndaelManaged rijndaelManager = new RijndaelManaged();
            rijndaelManager.Key = Encoding.UTF8.GetBytes(key.PadRight(32, ' ').Substring(0, 32));
            rijndaelManager.IV = AESIV;
            rijndaelManager.Mode = CipherMode.CBC;
            rijndaelManager.Padding = PaddingMode.PKCS7;
            ICryptoTransform rijndaelDecryptor = rijndaelManager.CreateDecryptor();

            try
            {
                byte[] decryptedData = rijndaelDecryptor.TransformFinalBlock(inputData, 0, inputData.Length);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                rijndaelDecryptor.Dispose();
                rijndaelManager.Clear();
            }
        }

        public static string AESEncrypt(string encryptString, string key)
        {
            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);

            RijndaelManaged rijndaelManager = new RijndaelManaged();
            rijndaelManager.Key = Encoding.UTF8.GetBytes(key.PadRight(32, ' ').Substring(0, 32));
            rijndaelManager.IV = AESIV;
            rijndaelManager.Mode = CipherMode.CBC;
            rijndaelManager.Padding = PaddingMode.PKCS7;

            ICryptoTransform rijndaelEncryptor = rijndaelManager.CreateEncryptor();

            byte[] encryptedData = rijndaelEncryptor.TransformFinalBlock(inputData, 0, inputData.Length);

            rijndaelEncryptor.Dispose();
            rijndaelManager.Clear();

            StringBuilder sb = new StringBuilder();
            foreach (byte b in encryptedData)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }

        public static string AESDecrypt(string decryptString, string key)
        {
            byte[] inputData = new byte[decryptString.Length / 2]; Convert.FromBase64String(decryptString);
            int arrayIndex = 0;
            foreach (byte b in inputData)
            {
                inputData[arrayIndex] = (byte)Convert.ToInt32(decryptString.Substring(arrayIndex * 2, 2), 16);
                arrayIndex = arrayIndex + 1;
            }

            RijndaelManaged rijndaelManager = new RijndaelManaged();
            rijndaelManager.Key = Encoding.UTF8.GetBytes(key.PadRight(32, ' ').Substring(0, 32));
            rijndaelManager.IV = AESIV;
            rijndaelManager.Mode = CipherMode.CBC;
            rijndaelManager.Padding = PaddingMode.PKCS7;
            ICryptoTransform rijndaelDecryptor = rijndaelManager.CreateDecryptor();

            try
            {
                byte[] decryptedData = rijndaelDecryptor.TransformFinalBlock(inputData, 0, inputData.Length);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                rijndaelDecryptor.Dispose();
                rijndaelManager.Clear();
            }
        }

    }
}
