using System;

namespace ML.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigFileNameAttribute : Attribute
    {

        public ConfigFileNameAttribute(string configName)
            : this(configName, Constant.DEFAULT_CONFIGPATHDIR, Constant.DEFAULT_EXTNAME, new DefaultConfigLevelProvider())
        {
        }

        public ConfigFileNameAttribute(string configName, IConfigLevelProvider cfgLevels)
            : this(configName, Constant.DEFAULT_CONFIGPATHDIR, Constant.DEFAULT_EXTNAME, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(string configName, string[] pathDirs)
            : this(configName, pathDirs, Constant.DEFAULT_EXTNAME, new DefaultConfigLevelProvider())
        {
        }

        public ConfigFileNameAttribute(string configName, string[] pathDirs, IConfigLevelProvider cfgLevels)
            : this(configName, pathDirs, Constant.DEFAULT_EXTNAME, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(string configName, string extName)
            : this(configName, Constant.DEFAULT_CONFIGPATHDIR, extName, new DefaultConfigLevelProvider())
        {
        }

        public ConfigFileNameAttribute(string configName, string extName, IConfigLevelProvider cfgLevels)
            : this(configName, Constant.DEFAULT_CONFIGPATHDIR, extName, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(string configName, string[] pathDirs, string extName, IConfigLevelProvider cfgLevels)
        {
            this.PathDirs = pathDirs;
            this.CfgName = configName;
            this.ExtName = "." + extName.TrimStart('.');
            this.CfgLevelsProvider = cfgLevels;
        }

        public string[] PathDirs { get; private set; }

        public string CfgName { get; private set; }
        public string ExtName { get; private set; }
        public string CfgFileName
        {
            get
            {
                return this.CfgName + this.ExtName;
            }
        }

        public IConfigLevelProvider CfgLevelsProvider { get; private set; }

    }
}
