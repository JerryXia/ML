using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Caching;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace ML.Configuration
{
    public class ConfigManager<T> where T : class, new()
    {
        private static List<ConfigLevelItem> _configLevelList;
        private static int _level;
        private static T _applicationConfig;
        private static int LoadCount = 0;

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
                LoadApplicationConfigs();
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
                _configLevelList = configLevelList; // 全局赋值
            }
            else
            {
                throw new ArgumentNullException("configLevelProvider.GetConfigLevels() result is null");
            }
        }

        private static void LoadApplicationConfigs()
        {
            T options = new T();
            ParseConfigTypeResult parseConfigTypeResult = ParseConfigFilePaths(options);

            var dictLoadResult = new ConcurrentDictionary<int, bool>();
            for (int i = 0; i < parseConfigTypeResult.ConfigFilePaths.Length; i++)
            {
                var loadCfgFileResult = LoadApplicationConfigsFromFile(parseConfigTypeResult.ConfigFilePaths[i], 
                    parseConfigTypeResult.CfgFileType);

                //dictLoadResult.TryAdd(i, loadCfgFileResult.HasError || loadCfgFileResult.FileExists == false);
                // 比如定的是Release的Level, release的配置文件不存在, 基础配置生不生效？
                dictLoadResult.TryAdd(i, loadCfgFileResult.HasError);
                if (loadCfgFileResult.FileExists)
                {
                    if (i == 0)
                    {
                        options = loadCfgFileResult.ConfigInstance;
                    }
                    else if (i == _level)
                    {
                        PropertyInfo[] pis0 = options.GetType().GetProperties();
                        PropertyInfo[] pis1 = loadCfgFileResult.ConfigInstance.GetType().GetProperties();
                        for (int j = 0; j < pis0.Length; j++)
                        {
                            if (pis0[j].CanWrite)
                            {
                                var newValue = GetPropertyValue(loadCfgFileResult.ConfigInstance, pis1, pis0[j].Name);
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
                        // Init操作调用LoadApplicationConfigs，检测默认文件是否存在，不存在则抛异常
                        throw new FileNotFoundException(string.Format("{0} Not Found", parseConfigTypeResult.ConfigFilePaths[i]));
                    }
                }
            }

            bool failed = false;
            if (dictLoadResult.TryGetValue(0, out failed))
            {
                if(failed)
                {
                    if (LoadCount == 0)
                    {
                        //init时发现失败，抛Exception
                        throw new Exception(string.Format("{0} Parse Error", parseConfigTypeResult.ConfigFilePaths[0]));
                    }
                    else
                    {
                        //后续出现Error, Ignore.
                    }
                }
                else
                {
                    if (dictLoadResult.TryGetValue(_level, out failed))
                    {
                        if (failed)
                        {
                            if (LoadCount == 0)
                            {
                                //init
                                throw new Exception(string.Format("{0} Parse Error", parseConfigTypeResult.ConfigFilePaths[0]));
                            }
                            else
                            {
                                //后续出现Error, Ignore.
                            }
                        }
                        else
                        {
                            _applicationConfig = options;
                        }
                    }
                }
            }

            Interlocked.Increment(ref LoadCount);
        }

        private static ParseConfigTypeResult ParseConfigFilePaths(T tEntity)
        {
            ParseConfigTypeResult result = new ParseConfigTypeResult();

            string configFilePath = null;
            List<ConfigLevelItem> extraSettings = null;

            Type entityType = tEntity.GetType();
            ConfigFileNameAttribute cfgFileNameAttr = (ConfigFileNameAttribute)Attribute.GetCustomAttribute(entityType, typeof(ConfigFileNameAttribute));
            if (cfgFileNameAttr == null)
            {
                result.CfgFileType = Constant.Default_ConfigFileType;

                // 找默认同名的配置文件
                configFilePath = CombineAppDataPaths(Constant.Default_RelativeDirNames,
                    String.Format("{0}.{1}", entityType.Name, Constant.Default_ConfigFileType.ToString().ToLower()));

                extraSettings = _configLevelList;

                string[] arr = new string[extraSettings.Count + 1];
                arr[0] = configFilePath;
                for (int i = 0; i < extraSettings.Count; i++)
                {
                    arr[i + 1] = CombineAppDataPaths(Constant.Default_RelativeDirNames, String.Format("{0}.{1}.{2}",
                        entityType.Name, extraSettings[i].ConfigName, Constant.Default_ConfigFileType.ToString().ToLower()));
                }
                result.ConfigFilePaths = arr;
            }
            else
            {
                result.CfgFileType = cfgFileNameAttr.Type;

                configFilePath = CombineAppDataPaths(cfgFileNameAttr.RelativeDirNames, cfgFileNameAttr.Name ?? entityType.Name);

                extraSettings = cfgFileNameAttr.LevelsProvider.GetConfigLevels();
                if (extraSettings != null && extraSettings.Count > 0)
                {
                    bool isExists = extraSettings.FindIndex(q => q.Index == 0) > -1;
                    if (isExists)
                    {
                        throw new Exception("IProjectConfigLevelProvider.GetConfigLevels() List<ProjectConfigLevelItem> can not has the default Index:0");
                    }
                }

                string[] arr = new string[extraSettings.Count + 1];
                arr[0] = configFilePath;
                for (int i = 0; i < extraSettings.Count; i++)
                {
                    arr[i + 1] = CombineAppDataPaths(cfgFileNameAttr.RelativeDirNames,
                        GenerateConfigFileName(cfgFileNameAttr, extraSettings[i].ConfigName));
                }
                result.ConfigFilePaths = arr;
            }

            return result;
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

        private static LoadApplicationConfigsResult<T> LoadApplicationConfigsFromFile(string configFilePath, ConfigFileType fileType)
        {
            var result = new LoadApplicationConfigsResult<T>();
            result.FileExists = File.Exists(configFilePath);
            if (result.FileExists)
            {
                try
                {
                    string content = ReadFromFile(configFilePath);

                    switch (fileType)
                    {
                        case ConfigFileType.Xml:
                            var encoding = Encoding.UTF8;
                            var xs = new XmlSerializer(typeof(T));
                            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(content)))
                            {
                                using (StreamReader sr = new StreamReader(ms, encoding))
                                {
                                    result.ConfigInstance = (T)xs.Deserialize(sr);
                                }
                            }
                            break;
                        case ConfigFileType.Json:
                            var jss = new JavaScriptSerializer();
                            result.ConfigInstance = jss.Deserialize<T>(content);
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception ex)
                {
                    result.HasError = true;
                    result.Error = ex;
                }
                finally
                {
                    
                }
            }

            var fileMinitor = new FileMonitor(configFilePath, new FileChangedHandler(ConfigFileChanged));
            fileMinitor.Init();

            return result;
        }

        private static void ConfigFileChanged(FileChangedEventArgs e)
        {
            // 由于事件发生时，文件可能还没有完全关闭，让程序等待50毫秒。
            System.Threading.Thread.Sleep(50);

            // 重新加载配置参数
            LoadApplicationConfigs();
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

        private static string GenerateConfigFileName(ConfigFileNameAttribute cfgFileNameAttr, string configLevelName)
        {
            bool hasPoint = cfgFileNameAttr.Name.IndexOf('.') > -1;
            if(hasPoint)
            {
                string[] tempArray = cfgFileNameAttr.Name.Split('.');
                string[] newArray = new string[tempArray.Length + 1];
                for (int i = 0; i < tempArray.Length; i++)
                {
                    if(i < tempArray.Length - 1)
                    {
                        newArray[i] = tempArray[i];
                    }
                    else
                    {
                        newArray[i] = configLevelName;
                    }
                }
                newArray[tempArray.Length] = tempArray[tempArray.Length - 1];

                return String.Join(".", newArray);
            }
            else
            {
                return String.Format("{0}.{1}", cfgFileNameAttr.Name, configLevelName);
            }
        }

        private static string ReadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                //var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //int bufferLen = (int)fs.Length;
                //byte[] buffers = new byte[bufferLen];
                //fs.Position = 0;
                //fs.Read(buffers, 0, bufferLen);
                //fs.Close();
                //fs.Dispose();
                //return Encoding.UTF8.GetString(buffers);
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            return string.Empty;
        }

    }
}
