namespace ML
{
    public class RealRandom : System.Random
    {
        private System.Random _random;

        public RealRandom()
            : this(GetRandomSeed())
        {
        }

        public RealRandom(int seed)
        {
            _random = new System.Random(seed);
        }

        /*
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
        */

        public static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return System.BitConverter.ToInt32(bytes, 0);
        }

    }
}
