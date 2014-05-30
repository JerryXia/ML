using System;
using System.IO;
using System.Collections;
using System.Text;

namespace ML.IO
{
    public class FileHelper
    {
        #region 文件操作

        /*******************************************************
        * 我们可以使用以下规则确定：
            1、如果应用程序在文件上执行几种操作，则使用FileInfo类更好一些，因为创建对象时，已经引用了正确的文件，而静态
        类每次都要寻找文件，会花费更多时间。
            2、如果进行单一的方法调用，则建议用File类，不必实例化对象。
        *******************************************************/


        /// <summary>
        /// 创建目录
        /// </summary>
        /// <returns>是否成功</returns>
        public static bool CreateDirectory(string dirPath)
        {
            if (String.IsNullOrEmpty(dirPath))
                return false;
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="recursive">是否删除子目录和文件</param>
        /// <returns>是否成功</returns>
        public static bool DeleteDirectory(string dirPath, bool recursive)
        {
            if (String.IsNullOrEmpty(dirPath))
                throw new ArgumentNullException("dirPath must not null");
            if (Directory.Exists(dirPath))
            {
                try
                {
                    Directory.Delete(dirPath, true);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static bool DeleteFile(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath must not null");
            FileInfo fileinfo = new FileInfo(filePath);
            if (fileinfo.Exists)
            {
                try
                {
                    fileinfo.Delete();
                    return true;
                }
                catch
                {
                    try
                    {
                        fileinfo.Refresh();
                        fileinfo.IsReadOnly = false;
                        fileinfo.Delete();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static void BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!File.Exists(sourceFileName))
            {
                throw new FileNotFoundException(sourceFileName + "File Not Exists");
            }
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        #endregion

        #region 其它操作

        public static string ReaderString(string fileName)
        {
            return ReaderString(fileName, Global.UTF8);
        }

        public static string ReaderString(string fileName, Encoding encode)
        {
            string result;
            StreamReader streamReader = new StreamReader(fileName, encode);
            result = streamReader.ReadToEnd();
            streamReader.Close();
            streamReader.Dispose();
            return result;
        }

        /// <summary>
        /// 写日志文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public static void WriteLog(string filePath, string content)
        {
            WriteLog(filePath, content, "yyyyMMdd hhmmss");
        }
        public static void WriteLog(string filePath, string content, string dateFormater)
        {
            StreamWriter sw;
            if (!File.Exists(filePath))
            {
                sw = File.CreateText(filePath);
            }
            else
            {
                sw = File.AppendText(filePath);
                sw.WriteLine("[" + DateTime.Now.ToString(dateFormater) + "]" + content);
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public static void WriteString(string filePath, string content)
        {
            StreamWriter sw;
            if (!File.Exists(filePath))
            {
                sw = File.CreateText(filePath);
            }
            else
            {
                sw = File.AppendText(filePath);
            }
            sw.Write(content);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        #endregion

    }
}