using System;
using System.Collections.Generic;
using System.Web;

namespace ML.Configuration
{
    public sealed class FileMonitor
    {
        private IList<string> _filePaths;
        private Action _cacheRemoveCallback;

        public FileMonitor(string filePath, Action fileChangeCallback)
        {
            _filePaths = new List<string> { filePath };
            _cacheRemoveCallback = fileChangeCallback;
        }

        public FileMonitor(IList<string> filePaths, Action fileChangeCallback)
        {
            _filePaths = filePaths;
            _cacheRemoveCallback = fileChangeCallback;
        }


        public void Init()
        {
            if (_cacheRemoveCallback == null)
            {
                throw new ArgumentNullException("CacheItemRemovedCallback can not be null");
            }

            if (_filePaths != null && _filePaths.Count > 0)
            {
                foreach (string item in _filePaths)
                {
                    FileWatchInit(item);
                }
            }
        }

        private void FileWatchInit(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string cacheKey = GenerateCacheKey();
                object cacheValue = GenerateCacheValue();
                var dep = new System.Web.Caching.CacheDependency(filePath);

                HttpRuntime.Cache.Insert(cacheKey, cacheValue, dep, 
                    System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.NotRemovable, CacheItemRemovedCallback);
            }
        }


        private string GenerateCacheKey()
        {
            return Guid.NewGuid().ToString();
        }

        private object GenerateCacheValue()
        {
            return new Object();
        }


        private void CacheItemRemovedCallback(string key, object value, System.Web.Caching.CacheItemRemovedReason reason)
        {
            /*
             System.Web.Caching.CacheItemRemovedReason.Removed:
             // 站点重启，应用程序池回收
             System.Web.Caching.CacheItemRemovedReason.Expired:
             // 正常时间到了
             */
            _cacheRemoveCallback();
        }

    }
}
