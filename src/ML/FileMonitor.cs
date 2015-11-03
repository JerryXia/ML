using System;
using System.Collections.Generic;
using System.Web;

namespace ML
{
    public delegate void FileChangedHandler(FileChangedEventArgs e);

    public sealed class FileMonitor
    {
        private IList<string> _filePaths;

        public event FileChangedHandler FileChanged;

        public FileMonitor(string filePath) : this(filePath, null) { }

        public FileMonitor(IList<string> filePaths) : this(filePaths, null) { }

        public FileMonitor(string filePath, FileChangedHandler fileChangeCallback)
        {
            _filePaths = new List<string> { filePath };
            if(fileChangeCallback != null)
            {
                FileChanged += fileChangeCallback;
            }
        }

        public FileMonitor(IList<string> filePaths, FileChangedHandler fileChangeCallback)
        {
            _filePaths = filePaths;
            if(fileChangeCallback != null)
            {
                FileChanged += fileChangeCallback;
            }
        }


        public void Init()
        {
            if (FileChanged == null)
            {
                throw new ArgumentNullException("FileChanged can not be null");
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
                var dep = new System.Web.Caching.CacheDependency(filePath);
                HttpRuntime.Cache.Insert(cacheKey, filePath, dep, 
                    System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.NotRemovable, CacheItemRemovedCallback);
            }
        }

        private string GenerateCacheKey()
        {
            return Guid.NewGuid().ToString();
        }

        private void CacheItemRemovedCallback(string key, object value, System.Web.Caching.CacheItemRemovedReason reason)
        {
            /*
             System.Web.Caching.CacheItemRemovedReason.Removed:
             // 站点重启，应用程序池回收
             System.Web.Caching.CacheItemRemovedReason.Expired:
             // 正常时间到了
             */
            RaiseFileChanged(value != null ? value.ToString() : string.Empty);
        }

        private void RaiseFileChanged(string value)
        {
            if (FileChanged != null)
            {
                FileChanged(new FileChangedEventArgs(value == null ? string.Empty : value));
            }
        }

    }

    public class FileChangedEventArgs : EventArgs
    {
        public FileChangedEventArgs(string fileFullName) : this(fileFullName, "") { }

        public FileChangedEventArgs(string fileFullName, string message)
        {
            this.FileFullName = fileFullName;
            this.Message = message;
        }

        public string Message { get; private set; }

        public string FileFullName { get; private set; }

    }

}
