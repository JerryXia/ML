using System;
using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace ML.Security.Cryptography
{
    public class BCEngine
    {
        private readonly Encoding _encoding;
        private readonly IBlockCipher _blockCipher;

        private IBlockCipherPadding _bcPadding;
        private PaddedBufferedBlockCipher _cipher;


        public BCEngine(IBlockCipher blockCipher, Encoding encoding)
        {
            _blockCipher = blockCipher;
            _encoding = encoding;
        }

        public void SetPadding(IBlockCipherPadding padding)
        {
            if (padding != null)
                _bcPadding = padding;
        }

        public string Encrypt(string plain, string key)
        {
            byte[] result = BouncyCastleCrypto(true, _encoding.GetBytes(plain), key);
            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipher, string key)
        {
            byte[] result = BouncyCastleCrypto(false, Convert.FromBase64String(cipher), key);
            return _encoding.GetString(result);
        }

        private byte[] BouncyCastleCrypto(bool forEncrypt, byte[] input, string key)
        {
            try
            {
                _cipher = _bcPadding == null ? new PaddedBufferedBlockCipher(_blockCipher) :
                    new PaddedBufferedBlockCipher(_blockCipher, _bcPadding);

                byte[] keyByte = _encoding.GetBytes(key);
                _cipher.Init(forEncrypt, new KeyParameter(keyByte));
                return _cipher.DoFinal(input);
            }
            catch (CryptoException ex)
            {
                throw new CryptoException("BCEngine CryptoException: ", ex);
            }
        }

    }
}
