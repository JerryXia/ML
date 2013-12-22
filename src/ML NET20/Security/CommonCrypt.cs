using System;
using System.Collections.Generic;
using System.Text;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;

namespace ML.Security
{
    public class CommonCrypt
    {
        #region Common

        static string CheckKeyLength(string key, int length)
        {
            return key.PadRight(length, ' ').Substring(0, length);
        }

        #endregion

        #region AES

        public static string AESEncryption(string plain, string key)
        {
            BCEngine bcEngine = new BCEngine(new AesEngine(), Global.UTF8);
            Pkcs7Padding padding = new Pkcs7Padding();
            padding.Init(new SecureRandom());
            bcEngine.SetPadding(padding);
            return bcEngine.Encrypt(plain, CheckKeyLength(key, 32));
        }

        public static string AESDecryption(string cipher, string key)
        {
            BCEngine bcEngine = new BCEngine(new AesEngine(), Global.UTF8);
            Pkcs7Padding padding = new Pkcs7Padding();
            padding.Init(new SecureRandom());
            bcEngine.SetPadding(padding);
            return bcEngine.Decrypt(cipher, CheckKeyLength(key, 32));
        }

        #endregion

        #region DES

        public static string DESEncryption(string plain, string key)
        {
            BCEngine bcEngine = new BCEngine(new DesEngine(), Global.UTF8);
            Pkcs7Padding padding = new Pkcs7Padding();
            padding.Init(new SecureRandom());
            bcEngine.SetPadding(padding);
            return bcEngine.Encrypt(plain, CheckKeyLength(key, 8));
        }

        public static string DESDecryption(string cipher, string key)
        {
            BCEngine bcEngine = new BCEngine(new DesEngine(), Global.UTF8);
            Pkcs7Padding padding = new Pkcs7Padding();
            padding.Init(new SecureRandom());
            bcEngine.SetPadding(padding);
            return bcEngine.Decrypt(cipher, CheckKeyLength(key, 8));
        }

        #endregion


    }
}
