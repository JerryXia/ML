namespace ML.Caching
{
    public sealed class CacheManager
    {
        public static ICache GetRuntimeCache()
        {
            return new RuntimeCache();
        }
        public static ICache GetRuntimeCache(double cacheTime)
        {
            return new RuntimeCache(cacheTime);
        }
        public static ICache GetRuntimeCache(CacheTime cacheTime)
        {
            return new RuntimeCache();
        }
        //public static ICache GetMemcachedCacher()
        //{
        //    return new MemcachedCacher();
        //}
        //public static ICache GetMemcachedCacher(string fileName)
        //{
        //    ICache result;
        //    if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileName.Trim()))
        //    {
        //        result = new MemcachedCacher();
        //    }
        //    else
        //    {
        //        result = new MemcachedCacher(fileName);
        //    }
        //    return result;
        //}
    }

    public enum CacheTime
    {
        /// <summary>
        /// 10 秒
        /// </summary>
        TenSecond = 10,

        /// <summary>
        /// 30 秒 
        /// </summary>
        ThirtySecond = 30,

        /// <summary>
        /// 1 分钟
        /// </summary>
        OneMinute = 60,

        /// <summary>
        /// 3 分钟
        /// </summary>
        ThreeMinute = 180,

        /// <summary>
        /// 5 分钟
        /// </summary>
        FiveMinute = 300,

        /// <summary>
        /// 7 分钟
        /// </summary>
        SevenMinute = 420,

        /// <summary>
        /// 10 分钟
        /// </summary>
        TenMinute = 600,

        /// <summary>
        /// 二十分钟
        /// </summary>
        TwentyMinute = 1200,

        /// <summary>
        /// 30 分钟
        /// </summary>
        ThirtyMinute = 1800,

        /// <summary>
        /// 1 小时
        /// </summary>
        OneHour = 3600,

        /// <summary>
        /// 5 小时
        /// </summary>
        FiveHour = 18000,

        /// <summary>
        /// 20 小时
        /// </summary>
        TwentyHour = 72000,

        /// <summary>
        /// 24 小时
        /// </summary>
        TwentyFourHour = 86400
    }
}
