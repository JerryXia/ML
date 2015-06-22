using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Caching;
using System.Web.Script.Serialization;

namespace ML.Configuration
{
    public class ConfigManager<T> where T : class, new()
    {
        private static List<ConfigLevelItem> _configLevelList;
        private static int _level;
        private static T _applicationConfig;

        public static T Current
        {
            get
            {
                return _applicationConfig;
            }
        }

        public static void Init(int level)
        {
            Init(level, new DefaultConfigLevelProvider());
        }

        public static void Init(int level, IConfigLevelProvider configLevelProvider)
        {
            LoadConfigLevels(configLevelProvider);

            if (_level != 0)
            {
                throw new Exception("you don't need to call the function: 'Init(int level)' again");
            }
            else
            {
                _level = level;
                _applicationConfig = LoadApplicationConfigs();
            }
        }


        private static void LoadConfigLevels(IConfigLevelProvider configLevelProvider)
        {
            if (configLevelProvider == null)
            {
                throw new ArgumentNullException("configLevelProvider can not be null");
            }

            var configLevelList = configLevelProvider.GetConfigLevels();
            Check(configLevelList);
        }

        private static void Check(List<ConfigLevelItem> configLevelList)
        {
            if (configLevelList != null && configLevelList.Count > 0)
            {
                // todo check
                _configLevelList = configLevelList;
            }
            else
            {
                throw new ArgumentNullException("configLevelProvider.GetConfigLevels() result is null");
            }
        }

        private static T LoadApplicationConfigs()
        {
            T options = new T();
            string[] configFilePaths = ParseConfigFilePaths(options);

            for (int i = 0; i < configFilePaths.Length; i++)
            {
                bool isExists = false;
                T t1 = LoadApplicationConfigsFromFile(configFilePaths[i], out isExists);

                if (isExists)
                {
                    if (i == 0)
                    {
                        options = t1;
                    }
                    else if (i == _level)
                    {
                        PropertyInfo[] pis0 = options.GetType().GetProperties();
                        PropertyInfo[] pis1 = t1.GetType().GetProperties();
                        for (int j = 0; j < pis0.Length; j++)
                        {
                            if (pis0[j].CanWrite)
                            {
                                var newValue = GetPropertyValue(t1, pis1, pis0[j].Name);
                                if (newValue != null)
                                {
                                    pis0[j].SetValue(options, newValue, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        //
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        // 检测默认文件是否存在，不存在则抛异常
                        throw new FileNotFoundException(string.Format("{0} Not Found", configFilePaths[i]));
                    }
                }
            }

            return options;
        }

        private static string[] ParseConfigFilePaths(T tEntity)
        {
            string configFilePath = null;
            List<ConfigLevelItem> extraSettings = null;

            Type entityType = tEntity.GetType();
            ConfigFileNameAttribute cfgNameAttr = (ConfigFileNameAttribute)Attribute.GetCustomAttribute(entityType, typeof(ConfigFileNameAttribute));
            if (cfgNameAttr == null)
            {
                // 找默认同名的配置文件
                configFilePath = CombineAppDataPaths(Constant.DEFAULT_CONFIGPATHDIR, entityType.Name + Constant.DEFAULT_EXTNAME);

                extraSettings = _configLevelList;

                string[] arr = new string[extraSettings.Count + 1];
                arr[0] = configFilePath;
                for (int i = 0; i < extraSettings.Count; i++)
                {
                    arr[i + 1] = CombineAppDataPaths(Constant.DEFAULT_CONFIGPATHDIR,
                        string.Format("{0}.{1}{2}", entityType.Name, extraSettings[i].ConfigName, Constant.DEFAULT_EXTNAME));
                }
                return arr;
            }
            else
            {
                configFilePath = CombineAppDataPaths(cfgNameAttr.PathDirs, cfgNameAttr.CfgFileName);

                extraSettings = cfgNameAttr.CfgLevelsProvider.GetConfigLevels();
                if (extraSettings != null && extraSettings.Count > 0)
                {
                    bool isExists = extraSettings.FindIndex(q => q.Index == 0) > -1;
                    if (isExists)
                    {
                        throw new Exception("IProjectConfigLevelProvider.GetConfigLevels() List<ProjectConfigLevelItem> can not has the default Index:1");
                    }
                }

                string[] arr = new string[extraSettings.Count + 1];
                arr[0] = configFilePath;
                for (int i = 0; i < extraSettings.Count; i++)
                {
                    arr[i + 1] = CombineAppDataPaths(cfgNameAttr.PathDirs, string.Format("{0}.{1}{2}",
                        cfgNameAttr.CfgName, extraSettings[i].ConfigName, cfgNameAttr.ExtName));
                }
                return arr;
            }
        }

        private static object GetPropertyValue(object instance, PropertyInfo[] pis1, string pName)
        {
            object propertyValue = null;
            for (int i = 0; i < pis1.Length; i++)
            {
                if (pis1[i].Name == pName)
                {
                    propertyValue = pis1[i].GetValue(instance, null);
                    break;
                }
            }
            return propertyValue;
        }

        private static T LoadApplicationConfigsFromFile(string configFilePath, out bool fileExists)
        {
            T options = new T();

            fileExists = File.Exists(configFilePath);
            if (fileExists)
            {
                string content = ReadFromFile(configFilePath);
                var jss = new JavaScriptSerializer();
                options = jss.Deserialize<T>(content);
            }

            var fileMinitor = new FileMonitor(new List<string>
            {
                configFilePath
            }, DefaultCacheRemovedCallback);
            fileMinitor.Init();

            return options;
        }

        private static void DefaultCacheRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            // 由于事件发生时，文件可能还没有完全关闭，让程序等待50毫秒。
            System.Threading.Thread.Sleep(50);

            // 重新加载配置参数
            _applicationConfig = LoadApplicationConfigs();
        }

        private static string CombineAppDataPaths(string[] paths, string fileName)
        {
            string[] pathsArr = (paths != null && paths.Length > 0) ? new string[paths.Length + 2] : new string[2];
            pathsArr[0] = Constant.Application_Dir;

            if (paths != null)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    pathsArr[i + 1] = paths[i];
                }
            }

            pathsArr[pathsArr.Length - 1] = fileName;
            return Path.Combine(pathsArr);
        }

        private static string ReadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                int bufferLen = (int)fs.Length;
                byte[] buffers = new byte[bufferLen];
                fs.Position = 0;
                fs.Read(buffers, 0, bufferLen);
                fs.Close();
                fs.Dispose();
                return Encoding.UTF8.GetString(buffers);
            }
            return string.Empty;
        }

    }
}
