using System;
using System.Security.Cryptography;
using System.Text;

namespace ML.Security
{
    public class AesCryptoService
    {
        //默认密钥向量
        static byte[] ORIGIN_IV = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

        private const CipherMode _cipherMode = CipherMode.CBC;
        private const PaddingMode _padding = PaddingMode.PKCS7;
        private const Char PADDING_CHAR = ' ';
        private const int KEY_LENGTH = 32;

        public static string EncryptBase64(string plainText, string key)
        {
            byte[] inputData = Encoding.UTF8.GetBytes(plainText);

            var aes = new AesManaged()
            {
                Padding = _padding,
                Mode = _cipherMode,
                IV = ORIGIN_IV,
                Key = Encoding.UTF8.GetBytes(key.PadRight(KEY_LENGTH, PADDING_CHAR).Substring(0, KEY_LENGTH))
            };

            var cryptor = aes.CreateEncryptor();
            var encryptedData = cryptor.TransformFinalBlock(inputData, 0, inputData.Length);

            cryptor.Dispose();
            aes.Clear();

            return Convert.ToBase64String(encryptedData);
        }

        public static bool DecryptBase64(string cliperText, string key, out string originText)
        {
            byte[] inputData = Convert.FromBase64String(cliperText);

            var aes = new AesManaged()
            {
                Padding = _padding,
                Mode = _cipherMode,
                IV = ORIGIN_IV,
                Key = Encoding.UTF8.GetBytes(key.PadRight(KEY_LENGTH, PADDING_CHAR).Substring(0, KEY_LENGTH))
            };

            var decryptor = aes.CreateDecryptor();
            bool decryptSuccess = true;
            try
            {
                byte[] decryptedData = decryptor.TransformFinalBlock(inputData, 0, inputData.Length);
                originText = Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                decryptSuccess = false;
                originText = string.Empty;
            }
            finally
            {
                decryptor.Dispose();
                if (aes != null)
                {
                    aes.Clear();
                }
            }
            return decryptSuccess;
        }

        public static string Encrypt(string plainText, string key)
        {
            byte[] inputData = Encoding.UTF8.GetBytes(plainText);

            var aes = new AesManaged()
            {
                Padding = _padding,
                Mode = _cipherMode,
                IV = ORIGIN_IV,
                Key = Encoding.UTF8.GetBytes(key.PadRight(KEY_LENGTH, PADDING_CHAR).Substring(0, KEY_LENGTH))
            };

            var cryptor = aes.CreateEncryptor();
            var encryptedData = cryptor.TransformFinalBlock(inputData, 0, inputData.Length);

            cryptor.Dispose();
            aes.Clear();

            StringBuilder sb = new StringBuilder();
            foreach (byte b in encryptedData)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        public static bool Decrypt(string cliperText, string key, out string originText)
        {
            int cliperTextLength = cliperText.Length;
            if (cliperTextLength % 2 != 0)
            {
                originText = String.Empty;
                return false;
            }

            byte[] inputData = new byte[cliperTextLength / 2];
            for (int i = 0; i < inputData.Length; i++)
            {
                inputData[i] = Convert.ToByte(cliperText.Substring(i * 2, 2), 16);
            }

            var aes = new AesManaged()
            {
                Padding = _padding,
                Mode = _cipherMode,
                IV = ORIGIN_IV,
                Key = Encoding.UTF8.GetBytes(key.PadRight(KEY_LENGTH, PADDING_CHAR).Substring(0, KEY_LENGTH))
            };

            var decryptor = aes.CreateDecryptor();
            bool decryptSuccess = true;
            try
            {
                byte[] decryptedData = decryptor.TransformFinalBlock(inputData, 0, inputData.Length);
                originText = Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                originText = String.Empty;
            }
            finally
            {
                decryptor.Dispose();
                if (aes != null)
                {
                    aes.Clear();
                }
            }
            return decryptSuccess;
        }

    }
}
