using System;

namespace ML.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigFileNameAttribute : Attribute
    {

        public ConfigFileNameAttribute(string configFileName)
            : this(configFileName, Constant.DEFAULT_CONFIGPATHDIR, ConfigFileType.Json, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string configFileName, string[] pathDirs)
            : this(configFileName, pathDirs, ConfigFileType.Json, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string configFileName, string[] pathDirs, ConfigFileType fileType)
            : this(configFileName, pathDirs, fileType, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string configFileName, string[] pathDirs, Type cfgLevels)
            : this(configFileName, pathDirs, ConfigFileType.Json, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(string configFileName, ConfigFileType fileType)
            : this(configFileName, Constant.DEFAULT_CONFIGPATHDIR, fileType, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string configFileName, ConfigFileType fileType, Type cfgLevels)
            : this(configFileName, Constant.DEFAULT_CONFIGPATHDIR, fileType, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(string configFileName, Type cfgLevels)
            : this(configFileName, Constant.DEFAULT_CONFIGPATHDIR, ConfigFileType.Json, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(string[] pathDirs, ConfigFileType fileType)
            : this(null, pathDirs, fileType, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string[] pathDirs, ConfigFileType fileType, Type cfgLevels)
            : this(null, pathDirs, fileType, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(string[] pathDirs, Type cfgLevels)
            : this(null, pathDirs, ConfigFileType.Json, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(ConfigFileType fileType, Type cfgLevels)
            : this(null, Constant.DEFAULT_CONFIGPATHDIR, fileType, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(string configFileName, string[] pathDirs, ConfigFileType fileType, Type cfgLevels)
        {
            if (String.IsNullOrWhiteSpace(configFileName))
            {
                throw new ArgumentNullException("configName can not be null or empty.");
            }

            switch (fileType)
            {
                case ConfigFileType.Xml:
                case ConfigFileType.Json:
                    //
                    break;
                case ConfigFileType.UnKnown:
                default:
                    throw new ArgumentNullException("fileType invalid.");
                    //break;
            }

            this.PathDirs = pathDirs;
            this.CfgFileName = configFileName;
            this.CfgFileType = fileType;

            bool isAssign = typeof(IConfigLevelProvider).IsAssignableFrom(cfgLevels);
            if (isAssign)
            {
                object obj = Activator.CreateInstance(cfgLevels);
                this.CfgLevelsProvider = obj as IConfigLevelProvider;
            }
            else
            {
                throw new ArgumentException("cfgLevels Type is invalid.");
            }
        }

        public string[] PathDirs { get; private set; }
        public string CfgFileName { get; private set; }
        public ConfigFileType CfgFileType { get; private set; }
        public IConfigLevelProvider CfgLevelsProvider { get; private set; }

    }
}
