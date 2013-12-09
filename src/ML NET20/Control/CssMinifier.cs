using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ML.Control
{
    /// <summary>
    /// CSS压缩工具
    /// </summary>
    internal class CssMinifier
    {
        /// <summary>
        /// 执行压缩
        /// </summary>
        public static string Execute(string src, string filePath)
        {
            string path = Regex.Replace(src, "[^/]*?$", "");
            string content = File.ReadAllText(filePath, Encoding.UTF8);

            content = Regex.Replace(content, @"\/\*.*?\*\/", "", RegexOptions.Singleline); //去注释
            content = Regex.Replace(content, @"\s*([;{])\s*\r?\n\s*", "$1");    //去样式块中换行
            content = Regex.Replace(content, @"\s*\r?\n\s*([}])\s*", "$1");     //去样式块后换行
            content = Regex.Replace(content, @":\s+", ":");     //去冒号后空格
            content = Regex.Replace(content, @"(\r?\n)\s+", "\r\n");     //去空行（含空字串）
            content = Regex.Replace(content, @"(\r?\n)\s+(\r?\n)", "\r\n");     //去空行（含空字串）
            content = Regex.Replace(content, @"(\r?\n)+", "\r\n");              //去空行
            content = Regex.Replace(content, @"url\([""']?(.*?)[""']?\)", m =>{ return string.Format("url({0})", ExcutePath(path, m.Groups[1].Value));}); //替换相对地址
            return content.Trim();
        }

        /// <summary>
        /// 计算绝对路径
        /// </summary>
        /// <param name="current">当前路径</param>
        /// <param name="path">资源相对路径</param>
        /// <returns></returns>
        static string ExcutePath(string current, string path)
        {
            //绝对地址不计算
            if (path.StartsWith("/")
                || path.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }
            string newpath = path;
            string parent = current.TrimEnd('/');
            while (Regex.IsMatch(newpath, @"^[./]"))
            {
                //当前目录
                if (newpath.StartsWith("./"))
                {
                    newpath = newpath.Remove(0, 2);
                }
                //上级目录
                if (newpath.StartsWith("../"))
                {
                    newpath = newpath.Remove(0, 3);
                    parent = Regex.Replace(parent, @"/[^/]+$", "");
                }
            }
            return parent + "/" + newpath;
        }
    }
}
