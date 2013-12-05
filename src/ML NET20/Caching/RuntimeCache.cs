using System;
using System.Web.Caching;

namespace ML.Caching
{
    internal sealed class RuntimeCache : ICache
    {
        private System.Web.Caching.Cache cache = System.Web.HttpRuntime.Cache;

        private double defaultCacheTime = 600;
        private System.Collections.Generic.List<object> cacheKeys;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal RuntimeCache() : this(600) { }

        internal RuntimeCache(CacheTime cacheTime) : this((double)cacheTime) { }

        internal RuntimeCache(double cacheTime)
        {
            this.defaultCacheTime = cacheTime;
            cacheKeys = new System.Collections.Generic.List<object>();
        }



        #region 相关属性

        /// <summary>
        /// 获取缓存的所有键key
        /// </summary>
        public System.Collections.Generic.List<object> CacheKeys
        {
            get
            {
                System.Collections.IDictionaryEnumerator cacheEnum = cache.GetEnumerator();
                while(cacheEnum.MoveNext())
                {
                    cacheKeys.Add(cacheEnum.Key);
                }
                return cacheKeys;
            }
        }

        /// <summary>
        /// 获和缓存总数
        /// </summary>
        public int Count
        {
            get
            {
                return this.cache == null ? 0 : this.cache.Count;
            }
        }

        /// <summary>
        /// 还可用的缓存百分比
        /// </summary>
        public long RemainMemoryPercentage
        {
            get
            {
                return cache.EffectivePercentagePhysicalMemoryLimit;
            }
        }

        /// <summary>
        /// 获取还可用于缓存的千字节数
        /// </summary>
        public long RemainMemoryBytes
        {
            get
            {
                return cache.EffectivePrivateBytesLimit;
            }
        }

        #endregion

        /// <summary>
        /// 是否存在缓存
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>true or false</returns>
        public bool Exists(string key)
        {
            return this.cache != null && this.cache.Get(key) != null;
        }

        /// <summary>
        /// 获得一个Cache对象
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns>value object</returns>
        public object Get(string key)
        {
            if (this.cache != null)
            {
                return this.cache.Get(key);
            }
            return null;
        }


        public void Insert(string key, object value)
        {
            Insert(key, value, null);
        }
        public void Insert(string key, object value, string filePath)
        {
            Insert(key, value, filePath, defaultCacheTime);
        }
        public void Insert(string key, object value, string filePath, double cacheTime)
        {
            Insert(key, value, filePath, cacheTime, Cache.NoSlidingExpiration);
        }
        public void Insert(string key, object value, string filePath, double cacheTime, TimeSpan slidingExpiration)
        {
            CacheDependency cacheDependency = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                cacheDependency = new CacheDependency(filePath);
            }
            //此处使用缓存对象绝对过期
            Insert(key, value, cacheDependency, DateTime.Now.AddSeconds(cacheTime), slidingExpiration, CacheItemPriority.Default, null);
        }


        public void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemUpdateCallback onUpdateCallback)
        {
            cache.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration, onUpdateCallback);
        }

        public void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            cache.Insert(key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
        }



        /// <summary>
        /// 删除一个Cache对象
        /// </summary>
        /// <param name="key">标识</param>
        public void Remove(string key)
        {
            if (this.cache != null)
            {
                cache.Remove(key);
            }
        }
 

    }
}
