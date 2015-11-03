using System;

namespace ML.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigFileNameAttribute : Attribute
    {
        // 可以直接不加特性，直接去默认值
        //public ConfigFileNameAttribute()
        //    : this(null, Constant.Default_RelativeDirNames, Constant.ConfigFile_DefaultType, typeof(DefaultConfigLevelProvider))
        //{
        //}

        public ConfigFileNameAttribute(string configFileName)
            : this(configFileName, Constant.Default_RelativeDirNames, Constant.Default_ConfigFileType, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string configFileName, string[] pathDirs)
            : this(configFileName, pathDirs, Constant.Default_ConfigFileType, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string configFileName, string[] pathDirs, ConfigFileType fileType)
            : this(configFileName, pathDirs, fileType, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string configFileName, string[] pathDirs, Type cfgLevels)
            : this(configFileName, pathDirs, Constant.Default_ConfigFileType, cfgLevels)
        {
        }

        public ConfigFileNameAttribute(string configFileName, ConfigFileType fileType)
            : this(configFileName, Constant.Default_RelativeDirNames, fileType, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string configFileName, ConfigFileType fileType, Type levelsProvider)
            : this(configFileName, Constant.Default_RelativeDirNames, fileType, levelsProvider)
        {
        }

        public ConfigFileNameAttribute(string configFileName, Type levelsProvider)
            : this(configFileName, Constant.Default_RelativeDirNames, Constant.Default_ConfigFileType, levelsProvider)
        {
        }

        public ConfigFileNameAttribute(string[] pathDirs)
            : this(null, pathDirs, Constant.Default_ConfigFileType, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string[] pathDirs, ConfigFileType fileType)
            : this(null, pathDirs, fileType, typeof(DefaultConfigLevelProvider))
        {
        }

        public ConfigFileNameAttribute(string[] pathDirs, ConfigFileType fileType, Type levelsProvider)
            : this(null, pathDirs, fileType, levelsProvider)
        {
        }

        public ConfigFileNameAttribute(string[] pathDirs, Type levelsProvider)
            : this(null, pathDirs, Constant.Default_ConfigFileType, levelsProvider)
        {
        }

        public ConfigFileNameAttribute(ConfigFileType fileType, Type levelsProvider)
            : this(null, Constant.Default_RelativeDirNames, fileType, levelsProvider)
        {
        }

        public ConfigFileNameAttribute(string configFileName, string[] pathDirs, ConfigFileType fileType, Type levelsProvider)
        {
            if (String.IsNullOrWhiteSpace(configFileName))
            {
                throw new ArgumentNullException("configFileName can not be null or empty.");
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

            this.RelativeDirNames = pathDirs;
            this.Name = configFileName;
            this.Type = fileType;

            bool isAssign = typeof(IConfigLevelProvider).IsAssignableFrom(levelsProvider);
            if (isAssign)
            {
                object obj = Activator.CreateInstance(levelsProvider);
                this.LevelsProvider = obj as IConfigLevelProvider;
            }
            else
            {
                throw new ArgumentException("LevelsProvider Type is invalid.");
            }
        }

        public string[] RelativeDirNames { get; private set; }
        public string Name { get; private set; }
        public ConfigFileType Type { get; private set; }
        public IConfigLevelProvider LevelsProvider { get; private set; }

    }
}
