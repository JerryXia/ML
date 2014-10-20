using System;

using System.Security.Cryptography;

namespace ML
{
    public class CustomRandom : Random
    {
        private Random _random;


        public CustomRandom() : this(GetRandomSeed())
        {
        }

        public CustomRandom(int seed)
        {
            _random = new Random(seed);
        }


        public override int Next()
        {
            return _random.Next();
        }
        public override int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }
        public override int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }
        public override double NextDouble()
        {
            return _random.NextDouble();
        }


        static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

    }
}
