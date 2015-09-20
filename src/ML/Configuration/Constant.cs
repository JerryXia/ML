using System;
using System.IO;

namespace ML.Configuration
{
    internal sealed class Constant
    {
        public static readonly string[] DEFAULT_CONFIGPATHDIR = { "App_Data", "Configs" };

        public static readonly string Application_Dir = 
            AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

        //public const string[] EmptyArray = { };

        //public static const char Point_Str = '.';
    }

    internal class ParseConfigTypeResult
    {
        public string[] ConfigFilePaths { get; set; }
        public ConfigFileType CfgFileType { get; set; }
    }

    internal class LoadApplicationConfigsResult<T> where T: class, new()
    {
        public LoadApplicationConfigsResult()
        {
            this.HasError = false;
            this.FileExists = false;
            //this.ConfigInstance = new T();
        }

        public T ConfigInstance { get; set; }
        public bool FileExists { get; set; }
        public bool HasError { get; set; }
        public Exception Error { get; set; }
    }

    public enum ConfigFileType
    {
        UnKnown = 0,

        Xml = 1,

        Json = 2
    }

}
