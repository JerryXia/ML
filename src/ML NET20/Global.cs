using System;
using System.IO;

namespace ML
{
    public class Global
    {
        #region 全局

        /// <summary>
        /// App and Web Directory
        /// </summary>
        public static string AppWebDir
        {
            get
            {
                //string s = System.Web.HttpRuntime.AppDomainAppPath;

                /*                             win  mac  unix
                 *   AltDirectorySeparatorChar  /    /     \
                 *   DirectorySeparatorChar     \    \     /
                 *   PathSeparator              ;           
                 *   VolumeSeparatorChar        :    :     /
                 */
                return System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[] { Path.DirectorySeparatorChar });
            }
        }

        public static string App_DataDir
        {
            get
            {
                return Path.Combine(AppWebDir, "App_Data");
            }
        }

        public static System.Text.Encoding UTF8 = System.Text.Encoding.UTF8;

        #endregion
    }
}
