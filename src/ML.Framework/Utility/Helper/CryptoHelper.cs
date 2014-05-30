using System.Text;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Security;

using ML.Security.Cryptography;

namespace ML.Framework.Utility
{
    public static class CryptoHelper
    {
        private static readonly AesFastEngine _aesFastEngine = new AesFastEngine();

        public static string AesEncrypt(string plain, string key)
        {
            var bcEngine = new BCEngine(_aesFastEngine, Encoding.UTF8);
            var padding = new Pkcs7Padding();
            padding.Init(new SecureRandom());
            bcEngine.SetPadding(padding);
            return bcEngine.Encrypt(plain, PadRightByLength(key, 32));
        }

        public static string AesDecrypt(string cipher, string key)
        {
            BCEngine bcEngine = new BCEngine(new AesFastEngine(), Encoding.UTF8);
            Pkcs7Padding padding = new Pkcs7Padding();
            padding.Init(new SecureRandom());
            bcEngine.SetPadding(padding);
            return bcEngine.Decrypt(cipher, PadRightByLength(key, 32));
        }

        static string PadRightByLength(string key, int length)
        {
            return key.PadRight(length, ' ').Substring(0, length);
        }

    }
}
