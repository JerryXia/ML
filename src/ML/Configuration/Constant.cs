using System;
using System.IO;

namespace ML.Configuration
{
    internal sealed class Constant
    {
        public const string DEFAULT_EXTNAME = ".json";

        public static readonly string[] DEFAULT_CONFIGPATHDIR = { "App_Data", "Configs" };

        public static readonly string Application_Dir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);


    }
}
