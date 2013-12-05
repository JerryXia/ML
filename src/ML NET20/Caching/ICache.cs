using System;
using System.Web.Caching;
namespace ML.Caching
{
    public interface ICache
    {
        System.Collections.Generic.List<object> CacheKeys { get; }
        int Count { get; }
        long RemainMemoryPercentage { get; }
        long RemainMemoryBytes { get; }
        bool Exists(string key);
        object Get(string key);
        void Insert(string key, object value);
        void Insert(string key, object value, string filePath);
        void Insert(string key, object value, string filePath, double cacheTime);
        void Insert(string key, object value, string filePath, double cacheTime, System.TimeSpan slidingExpiration);
        void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemUpdateCallback onUpdateCallback);
        void Insert(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback);
        void Remove(string key);
    }
}
