using System.Web;

namespace ML.Config
{
    public class Global
    {
        #region App and Web Directory

        /// <summary>
        /// App的目录
        /// </summary>
        public static string AppDir
        {
            get
            {
                return System.Environment.CurrentDirectory;
            }
        }
        /// <summary>
        /// Web应用程序的目录
        /// </summary>
        public static string WebDir
        {
            get
            {
                string s = HttpRuntime.AppDomainAppPath;
                return System.AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        #endregion
    }
}
