using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace ML.Caching
{
    public static class CacheHelper
    {
        #region 常量

        static readonly MemoryCache _cacheDefault = MemoryCache.Default;
        static CacheItemPolicy policy;

        /// <summary>
        /// 获取一个值，该值指示某个缓存项没有绝对过期。
        /// 返回 已设置为可能的最大值的日期时间值。
        /// </summary>
        public static readonly DateTimeOffset InfiniteAbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration;
        /// <summary>
        /// 指示某个缓存项没有可调过期时间。
        /// 返回 设置为零的持续时间值。
        /// </summary>
        public static readonly TimeSpan NoSlidingExpiration = ObjectCache.NoSlidingExpiration;

        #endregion

        public static bool Contains(string key)
        {
            return _cacheDefault.Contains(key);
        }

        public static object Get(string key)
        {
            return _cacheDefault.Get(key);
        }
        public static CacheItem GetCacheItem(string key)
        {
            return _cacheDefault.GetCacheItem(key);
        }

        /// <summary>
        /// Custom Set
        /// </summary>
        public static void Set(string key, object value, double minutes)
        {
            DateTimeOffset absoluteExpiration = SetRandomTime(minutes);

            policy = GenerateCacheItemPolicy(absoluteExpiration, NoSlidingExpiration, CacheItemPriority.Default);

            Set(key, value, policy);
        }
        /// <summary>
        /// 在默认时间上加上随机秒数
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        static DateTimeOffset SetRandomTime(double minutes)
        {
            TimeSpan timespan = TimeSpan.FromMinutes(minutes);

            double seconds = timespan.TotalSeconds / 10;
            if (seconds > 1800.00)
            {
                seconds = 1800.00;
            }
            seconds = new Random().NextDouble() * seconds;

            timespan = timespan + TimeSpan.FromSeconds(seconds);
            return DateTimeOffset.Now.Add(timespan);
        }
        static CacheItemPolicy GenerateCacheItemPolicy(DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration, CacheEntryUpdateCallback onUpdateCallback = null)
        {
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;
            policy.SlidingExpiration = slidingExpiration;
            policy.UpdateCallback = onUpdateCallback;
            return policy;
        }
        static CacheItemPolicy GenerateCacheItemPolicy(DateTimeOffset absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheEntryRemovedCallback onRemoveCallback = null)
        {
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;
            policy.SlidingExpiration = slidingExpiration;
            policy.Priority = priority;
            policy.RemovedCallback = onRemoveCallback;
            return policy;
        }


        public static void Set(CacheItem item, CacheItemPolicy policy)
        {
            _cacheDefault.Set(item, policy);
        }
        public static void Set(string key, object value, CacheItemPolicy policy)
        {
            _cacheDefault.Set(key, value, policy);
        }
        public static void Set(string key, object value, DateTimeOffset absoluteExpiration)
        {
            _cacheDefault.Set(key, value, absoluteExpiration);
        }

        public static object Remove(string key)
        {
            return _cacheDefault.Remove(key);
        }

        public static long Trim(int percent)
        {
            return _cacheDefault.Trim(percent);
        }

        public static CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys)
        {
            return _cacheDefault.CreateCacheEntryChangeMonitor(keys);
        }

        public static IDictionary<string, object> GetValues(IEnumerable<string> keys)
        {
            return _cacheDefault.GetValues(keys);
        }

        #region 缓存统计

        /// <summary>
        /// 返回缓存中的缓存项总数
        /// </summary>
        public static long Count
        {
            get
            {
                return _cacheDefault.GetCount();
            }
        }

        /// <summary>
        /// 获取计算机上缓存可使用的内存量（以兆字节为单位）
        /// </summary>
        public static long CacheMemoryLimit
        {
            get
            {
                return _cacheDefault.CacheMemoryLimit;
            }
        }

        /// <summary>
        /// 获取缓存可使用的物理内存的百分比
        /// </summary>
        public static long PhysicalMemoryLimit
        {
            get
            {
                return _cacheDefault.PhysicalMemoryLimit;
            }
        }

        /// <summary>
        /// 获取在缓存更新其内存统计信息之前需等待的最大时间量。
        /// </summary>
        public static TimeSpan PollingInterval
        {
            get
            {
                return _cacheDefault.PollingInterval;
            }
        }

        /// <summary>
        /// Cache Key的集合
        /// </summary>
        public static List<string> CacheKeys
        {
            get
            {
                return _cacheDefault.Select(kvp => kvp.Key).ToList();
            }
        }

        #endregion
    }
}
