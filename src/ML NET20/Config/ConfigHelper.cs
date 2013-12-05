using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace ML.Config
{
    /// <summary>
    /// 自定义的系统参数配置文件的读取工具类
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 取~/Config/CommonConfig.xml中某个参数名对应的参数值
        /// </summary>
        /// <param name="ParameterName">参数名</param>
        /// <returns>参数值</returns>
        public static string GetParameterValue(string ParameterName)
        {
            return GetParameterValue("EpowerConfig", ParameterName);
        }

        /// <summary>
        /// 取某个参数配置文件中某个参数名对应的参数值
        /// 参数配置文件
        ///        1、必须存放于"~/Config/"目录下面，以.xml为后缀
        ///        2、配置格式参见~/Config/CommonConfig.xml
        /// </summary>
        /// <param name="ConfigName">配置文件的文件名，不要后缀</param>
        /// <param name="ParameterName">参数名</param>
        /// <returns>参数值</returns>
        public static string GetParameterValue(string ConfigName, string ParameterName)
        {
            Hashtable CommonConfig = GetConfigCache(ConfigName);

            if (CommonConfig.ContainsKey(ParameterName))
                return CommonConfig[ParameterName].ToString();
            else
                throw new Exception("参数(" + ParameterName + ")没有定义，请检查配置文件！");
        }

        /// <summary>
        /// 将配置的参数转换成Hashtable并存入缓存，配置文件修改后自动更新缓存
        /// </summary>
        /// <param name="ConfigName"></param>
        /// <returns></returns>
        private static Hashtable GetConfigCache(string ConfigName)
        {
            string CacheName = "Config_" + ConfigName;

            Hashtable CommonConfig = new Hashtable();
            Cache cache = HttpRuntime.Cache;

            if (cache[CacheName] == null)
            {
                string ConfigPath = null;

                #region 取应用程序根物理路径
                try
                {
                    ConfigPath = HttpRuntime.AppDomainAppPath;
                }
                catch (System.ArgumentException ex)
                {
                }

                if (ConfigPath == null) throw new Exception("系统异常，取不到应用程序所在根物理路径！");
                #endregion

                string ConfigFile = ConfigPath + "Config\\" + ConfigName + ".xml";

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(ConfigFile);

                XmlNode oNode = xmlDoc.DocumentElement;

                if (oNode.HasChildNodes)
                {
                    XmlNodeList oList = oNode.ChildNodes;

                    for (int i = 0; i < oList.Count; i++)
                    {
                        CommonConfig.Add(oList[i].Attributes["Name"].Value, oList[i].Attributes["Value"].Value);
                    }
                }

                cache.Insert(CacheName, CommonConfig, new CacheDependency(ConfigFile));
            }
            else
                CommonConfig = (Hashtable)cache[CacheName];

            return CommonConfig;
        }
    }
}
