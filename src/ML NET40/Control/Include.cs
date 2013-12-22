using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ML.Control
{
    /// <summary>
    /// Js, Css 脚本合并，压缩，并更新时间戳，前端版本控制
    /// </summary>
    [PersistChildren(true)]
    public class Include : System.Web.UI.Control
    {

        // 合并的输出目录 (如果不存在, 会自动创建)
        static string JsOutputDir = "~/assets/js";
        static string CssOutputDir = "~/assets/css";

        // 是否需要压缩脚本文件, 默认需要
        bool compress = true;

        public bool Compress
        {
            get { return compress; }
            set { compress = value; }
        }

        static Include()
        {
            JsOutputDir = ConfigurationManager.AppSettings["jsoutput"] ?? JsOutputDir;
            CssOutputDir = ConfigurationManager.AppSettings["cssoutput"] ?? CssOutputDir;
        }

        // 输出Html脚本块
        protected override void Render(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();
            HtmlTextWriter hw = new HtmlTextWriter(new StringWriter(sb));
            // 先获取子项的内容
            this.RenderChildren(hw);
            string content = sb.ToString();
            if (this.compress)
            {
                content = GetContent(content, writer);
            }

            if (!String.IsNullOrEmpty(content))
            {
                writer.Write(content);
            }
        }

        // 按照配置, 返回合并后的脚本文件块 
        protected string GetContent(string content, HtmlTextWriter writer)
        {
            if (String.IsNullOrEmpty(content))
            {
                return "";
            }

            string cssBlock = BlockHelper.GetCssBlock(content, Context.Response.ApplyAppPathModifier(CssOutputDir), this.compress);
            string jsBlock = BlockHelper.GetJsBlock(content, Context.Response.ApplyAppPathModifier(JsOutputDir), this.compress);
            if (String.IsNullOrEmpty(cssBlock) && String.IsNullOrEmpty(jsBlock))
            {
                return "";
            }

            // 如果2个脚本块都有，则各起一行
            if (!String.IsNullOrEmpty(cssBlock) && !String.IsNullOrEmpty(jsBlock))
            {
                jsBlock = writer.NewLine + jsBlock;
            }

            return cssBlock + jsBlock;
        }

    }


    #region -- 引用文件

    /// <summary>
    /// 引用文件
    /// </summary>
    class IncludeFile
    {
        string src;
        string locationFile;
        DateTime lastWriteTime = DateTime.MinValue;

        public string Src
        {
            get { return src; }
        }

        public string LocationFile
        {
            get { return locationFile; }
        }

        public DateTime LastWriteTime
        {
            get
            {
                if (lastWriteTime == DateTime.MinValue)
                {
                    this.lastWriteTime = File.GetLastWriteTime(this.locationFile);
                }
                return this.lastWriteTime;
            }
        }

        public string Version
        {
            get
            {
                return LastWriteTime.Ticks.ToString();
            }
        }

        public IncludeFile(string src, string locationFile)
        {
            this.src = src;
            this.locationFile = locationFile;
        }
    }

    #endregion

    #region -- 脚本块操作

    /// <summary>
    /// 引用块操作工具
    /// </summary>
    class BlockHelper
    {
        // 是否合并CSS, JS文件
        const bool MergeCssFile = true;
        const bool MergeJsFile = true;

        readonly HttpServerUtility Server = HttpContext.Current.Server;
        readonly HttpRequest Request = HttpContext.Current.Request;
        readonly HttpResponse Response = HttpContext.Current.Response;

        string fileType;			// 文件类型, 如: js, css 
        bool compress;				// 是否压缩 (现在只有js支持压缩)
        bool merge;					// 是否合并
        string content;				// 文本块内容
        List<IncludeFile> files;	// 文本块包含的文件列表
        string outputDir;			// 输出目录
        IncludeFile outputFile;		// 输出文件(压缩合并后的文件)

        // 获得CSS输出
        public static string GetCssBlock(string content, string outputDir, bool compress)
        {
            return new BlockHelper(content, outputDir, "css", compress, MergeCssFile).GetBlock();
        }

        // 获得JS输出
        public static string GetJsBlock(string content, string outputDir, bool compress)
        {
            return new BlockHelper(content, outputDir, "js", compress, MergeJsFile).GetBlock();
        }

        public BlockHelper(string content, string outputDir, string fileType, bool compress, bool merge)
        {
            this.content = content;
            this.outputDir = outputDir;
            this.fileType = fileType;
            this.compress = compress;
            this.merge = merge;
            this.files = GetFiles();
            this.outputFile = GetOutputFile();

            // 如果输出目录不存在，则创建
            if (!Directory.Exists(Server.MapPath(this.outputDir)))
            {
                Directory.CreateDirectory(Server.MapPath(this.outputDir));
            }
        }

        // 获得输出文件的相关信息
        IncludeFile GetOutputFile()
        {
            if (this.files.Count == 0)
            {
                return null;
            }

            // 拼接包含的文件名,包含路径信息
            string name = "";
            foreach (IncludeFile file in this.files)
            {
                name += "$" + file.Src.Replace("/", "_").Replace(".", "_");
            }
            name = ToMD5(name);
            name += "." + this.fileType;

            string src = outputDir.TrimEnd('/') + "/" + name;
            string locationFile = Server.MapPath(src);
            return new IncludeFile(src, locationFile);
        }

        public static string ToMD5(string str)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        // 根据文件类型, 获得文本块包含的文件列表
        List<IncludeFile> GetFiles()
        {

            // 从文件后缀往前找
            // <link rel="stylesheet" href="/css/base.css" type="text/css"/>
            //                              ~~~~~~~~~^^^^      
            string pattern = @"[""']([^""']*?\." + this.fileType + ")";
            MatchCollection ms = Regex.Matches(this.content, pattern, RegexOptions.IgnoreCase);

            List<IncludeFile> files = new List<IncludeFile>();
            foreach (Match m in ms)
            {
                string value = m.Groups[1].Value;
                value = value.Trim().ToLower();
                try
                {
                    // 暂时只考虑本站点的文件
                    string fileName = Server.MapPath(value);
                    if (File.Exists(fileName))
                    {
                        files.Add(new IncludeFile(Response.ApplyAppPathModifier(value), Server.MapPath(value)));
                    }
                }
                catch
                {
                }
            }
            return files;
        }

        // 获得输出的脚本块
        public string GetBlock()
        {
            if (this.files.Count == 0)
            {
                return "";
            }

            if (this.merge)
            {
                if (NeedUpdate())
                {
                    WriteMergeFile();
                }
                // 需要合并, 则输出合并后的文件
                return GetBlock(this.outputFile);
            }
            else
            {
                // 否则只是在文件列表末尾加时间戳
                return GetBlock(this.files);
            }
        }

        // 是否需要更新
        bool NeedUpdate()
        {
            // 如果输出文件不存在
            if (!File.Exists(this.outputFile.LocationFile))
            {
                return true;
            }

            // 如果输出文件在要合并的脚本文件之前创建
            DateTime outputFileLastWriteTime = this.outputFile.LastWriteTime;
            foreach (IncludeFile file in files)
            {
                if (outputFileLastWriteTime < file.LastWriteTime)
                {
                    return true;
                }
            }

            // 否则不用更新
            return false;
        }

        // 合并文件
        void WriteMergeFile()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IncludeFile file in this.files)
            {
                // 目前只压缩JS文件
                if (this.fileType == "js")
                {
                    sb.AppendLine("//org:" + file.Src);
                    sb.AppendLine(JavaScriptMinifier.Execute(file.LocationFile));
                }
                else if (this.fileType == "css")
                {
                    sb.AppendLine("/*org:" + file.Src + "*/");
                    sb.AppendLine(CssMinifier.Execute(file.Src, file.LocationFile));
                }
                else
                {
                    sb.AppendLine(File.ReadAllText(file.LocationFile, Encoding.UTF8));
                }
            }
            File.WriteAllText(this.outputFile.LocationFile, sb.ToString());
        }

        // 按照类型, 输出JS或CSS脚本块
        string GetBlock(IncludeFile file)
        {
            string template = "";
            if (this.fileType == "js")
            {
                template = "<script type=\"text/javascript\" src=\"{0}?v={1}\"></script>";
            }
            if (this.fileType == "css")
            {
                template = "<link type=\"text/css\" href=\"{0}?v={1}\" rel=\"stylesheet\" />";
            }
            return string.Format(template, file.Src, file.Version);
        }

        string GetBlock(List<IncludeFile> fileList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IncludeFile file in fileList)
            {
                sb.AppendLine(GetBlock(file));
            }
            return sb.ToString();
        }
    }

    #endregion
}
