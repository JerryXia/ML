using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace ML.Configuration
{
    internal sealed class FileMonitor
    {
        private IList<string> _filePaths;
        private CacheItemRemovedCallback _cacheRemoveCallback;

        public FileMonitor(IList<string> filePaths, CacheItemRemovedCallback cacheRemoveCallback)
        {
            _filePaths = filePaths;
            _cacheRemoveCallback = cacheRemoveCallback;
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
                var dep = new CacheDependency(filePath);

                HttpRuntime.Cache.Insert(cacheKey, cacheValue, dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration,
                    CacheItemPriority.NotRemovable, _cacheRemoveCallback);
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
    }
}
