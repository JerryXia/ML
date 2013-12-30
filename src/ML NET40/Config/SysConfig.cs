using System;

namespace ML.Config
{
    public class SysConfig
    {
        public static string GetConfigVal(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key must not be null");
            }
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public static int GetConfigIntVal(string key)
        {
            int i = 0;
            Int32.TryParse(GetConfigVal(key), out i);
            return i;
        }

        public static double GetConfigDoubleVal(string key)
        {
            double i = 0.00;
            Double.TryParse(GetConfigVal(key), out i);
            return i;
        }

        public static string GetConnectionString(string key)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        public static object GetSection(string key)
        {
            return System.Configuration.ConfigurationManager.GetSection(key);
        }
    }
}
