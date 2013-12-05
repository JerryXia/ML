using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;

using ML.Xml;

namespace ML.Config
{
    public class CustomXmlConfig
    {
        const string CacheDependencyValueObj = "JerryXia-CustomXmlConfig";


        private readonly string fileName;
        private readonly string optionsCacheKey;
        private int s_OptionsCacheDependencyFlag;
        private IRunOptions _options;

        public CustomXmlConfig(string xmlFileName)
        {
            this.fileName = xmlFileName;
            this.optionsCacheKey = Convert.ToString(Guid.NewGuid());
        }

        public IRunOptions Options
        {
            get { return this._options; }
        }


        public IRunOptions LoadRunOptions()
        {
            string xmlPath = Path.Combine(Global.App_DataDir, fileName);

            IRunOptions options = XmlSerialization.XmlDeserializeFromFile<IRunOptions>(xmlPath, Encoding.UTF8);

            int flag = System.Threading.Interlocked.CompareExchange(ref s_OptionsCacheDependencyFlag, 1, 0);
            if (flag == 0)
            {
                //Monitoring this option file
                CacheDependency dependency = new CacheDependency(xmlPath);
                HttpRuntime.Cache.Insert(optionsCacheKey, CacheDependencyValueObj, dependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, RunOptionsUpdateCallback);
            }

            return options;
        }

        public void RunOptionsUpdateCallback(string key, CacheItemUpdateReason reason, out object expensiveObject, out CacheDependency dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
        {
            // 注意哦：在这个方法中，不要出现【未处理异常】，否则缓存对象将被移除。

            // 说明：这里并不关心参数reason，因为我根本就没有使用过期时间  所以，只有一种原因：依赖的文件发生了改变。

            expensiveObject = CacheDependencyValueObj;
            dependency = new CacheDependency(Path.Combine(Global.App_DataDir, fileName));
            absoluteExpiration = Cache.NoAbsoluteExpiration;
            slidingExpiration = Cache.NoSlidingExpiration;

            // 由于事件发生时，文件可能还没有完全关闭，所以只好让程序稍等。
            System.Threading.Thread.Sleep(50);

            // 重新加载配置参数
            _options = LoadRunOptions();
        }

    }

    public interface IRunOptions
    {

    }
}
