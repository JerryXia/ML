using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ML.Utility
{
    /// <summary>
    /// 文字处理工具类
    /// </summary>
    public class TextHelper
    {
        /// <summary>
        /// 检查URL,带http://
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CheckForUrl(string text)
        {
            if (text == null || text.Trim().Length == 0 || text.Trim().ToLower().StartsWith("http://"))
                return text;
            else
                return "http://" + text;
        }


        #region 私有正则式
        // html 过滤器
        private static Regex regexHtmlCode = new Regex(@"<[^>]*>", RegexOptions.IgnoreCase);
        // ubb 过滤器
        private static Regex regexUbbCode = new Regex(@"\[(?<x>[^\]]*)\]", RegexOptions.IgnoreCase);
        // 脚本 过滤器
        private static Regex regexScript = new Regex(@"(?i)<script([^>])*>(\w|\W)*</script([^>])*>", RegexOptions.IgnoreCase);
        private static Regex regexScriptHeader = new Regex(@"<script([^>])*>", RegexOptions.IgnoreCase);
        private static Regex regexScriptFooter = new Regex(@"</script>", RegexOptions.IgnoreCase);
        // iframe 过滤器
        private static Regex regexIFrame = new Regex(@"(?i)<iframe([^>])*>(\w|\W)*</iframe([^>])*>", RegexOptions.IgnoreCase);
        private static Regex regexIframeHeader = new Regex("<iframe([^>])*>", RegexOptions.IgnoreCase);
        private static Regex regexIframeFooter = new Regex("</iframe>", RegexOptions.IgnoreCase);
        // <*> 过滤器
        private static Regex regexAngularBracket = new Regex(@"<[^>]*>", RegexOptions.IgnoreCase);
        // url 正则式
        private static Regex regexUrl = new Regex(@"(^|<br>|<br/>|[^('|\>|""|=|&quot;|&gt;)])((?:http|ftp|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        // space正则
        private static Regex regexSpace = new Regex("(?<fore>(?:(?:[^< ])*(?:<(?:!--(?:(?:[^-])*(?:(?=-->)|-))*--|(?:[^>])+)>)?)*)[ ](?<back>(?:(?:[^< ])*(?:<(?:!--(?:(?:[^-])*(?:(?=-->)|-))*--|(?:[^>])+)>)?)*)", RegexOptions.IgnoreCase);
        // 数字正则检查
        private static Regex regexNumber = new Regex("^\\d+$");
        // sql 注入检查
        private static Regex regexSql = new Regex(@"select |insert |delete from |count\(|drop table|update |truncate |asc\(|mid\(|char\(|xp_cmdshell|exec master|net localgroup administrators|:|net user|""|\'| or ", RegexOptions.IgnoreCase);
        // 空格 
        private static Regex regexBlank = new Regex(@"\s");

        private static Regex regexUbbB = new Regex(@"\[b\](?<x>[^\]]*)\[/b\]", RegexOptions.IgnoreCase);
        private static Regex regexUbbI = new Regex(@"\[i\](?<x>[^\]]*)\[/i\]", RegexOptions.IgnoreCase);
        private static Regex regexUbbU = new Regex(@"\[u\](?<x>[^\]]*)\[/u\]", RegexOptions.IgnoreCase);
        private static Regex regexUbbImg = new Regex(@"\[img\]((?:http)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?)\[/img\]", RegexOptions.IgnoreCase);
        private static Regex regexUbbImg2 = new Regex(@"\[img url=((?:http)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?)\]((?:http)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?)\[/img\]", RegexOptions.IgnoreCase);
        private static Regex regexUbbUrl = new Regex(@"\[url\]((?:http)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?)\[/url\]", RegexOptions.IgnoreCase);
        private static Regex regexUbbUrl2 = new Regex(@"\[url=((?:http)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?)\](.*?)\[/url\]", RegexOptions.IgnoreCase);
        private static Regex regexAlign = new Regex(@"\[align=(?<x>[^\]]*)\](?<y>[^\]]*)\[/align\]", RegexOptions.IgnoreCase);
        private static Regex regexColor = new Regex(@"\[color=(?<x>[^\]]*)\](?<y>[^\]]*)\[/color\]", RegexOptions.IgnoreCase);
        private static Regex regexFace = new Regex(@"\[face=(.*?)\](.*?)\[/face\]", RegexOptions.IgnoreCase);
        private static Regex regexFlv = new Regex(@"\[flv\](.*?)\[/flv\]", RegexOptions.IgnoreCase);
        private static Regex regexMp3 = new Regex(@"\[mp3\](.*?)\[/mp3\]", RegexOptions.IgnoreCase);
        private static Regex regexIframeWH = new Regex(@"\[iframe=(\d*),(\d*)\](.*?)\[/iframe\]", RegexOptions.IgnoreCase);
        private static Regex regexVote = new Regex(@"\[vote=(\d*),(\d*)\](.*?)\[/vote\]", RegexOptions.IgnoreCase);
        private static Regex regexHJPlay1 = new Regex(@"\[hjp=(\d*),(\d*),((false|true))\](.*?)\[/hjp\]", RegexOptions.IgnoreCase);
        private static Regex regexHJPlay2 = new Regex(@"\[hjp2=(\d*),(\d*),((false|true))\](.*?)\[/hjp2\]", RegexOptions.IgnoreCase);
        private static Regex regexHJPlay3 = new Regex(@"\[hjp3\]hjptype=song&amp;player=1&amp;son=(.*?)&amp;autoplay=no&amp;caption=false&amp;lrc=&amp;autoreplay=1&amp;bgcolor=FFFFFF&amp;width=(\d*)&amp;height=(\d*)\[/hjp3\]", RegexOptions.IgnoreCase);
        private static Regex regexHJPlay32 = new Regex(@"\[hjp3\](.*?)\[/hjp3\]", RegexOptions.IgnoreCase);
        private static Regex regexQuote = new Regex(@"\[quote\](.*?)\[/quote\]", RegexOptions.IgnoreCase);
        private static Regex regexReplyView = new Regex(@"\[replyview\](.*?)\[/replyview\]", RegexOptions.IgnoreCase);
        private static Regex regexD = new Regex(@"\[w\](.*?)\[/w\]", RegexOptions.IgnoreCase);
        private static Regex regexD2 = new Regex(@"\[w=(.*?)\](.*?)\[/w\]", RegexOptions.IgnoreCase);
        private static Regex regexD3 = new Regex(@"\[wv\](.*?)\[/wv\]", RegexOptions.IgnoreCase);
        private static Regex regexDJ = new Regex(@"\[wj\](.*?)\[/wj\]", RegexOptions.IgnoreCase);
        private static Regex regexDJ2 = new Regex(@"\[wj=(.*?),(.*?)\](.*?)\[/wj\]", RegexOptions.IgnoreCase);
        private static Regex regexDJ3 = new Regex(@"\[wjv=(.*?)\](.*?)\[/wjv\]", RegexOptions.IgnoreCase);
        private static Regex regexFlash = new Regex(@"\[flash=(\d*),(\d*)\](\S*)\[/flash\]", RegexOptions.IgnoreCase);
        private static Regex regexIPA = new Regex(@"\[ipa\](?<x>[^\]]*)\[/ipa\]", RegexOptions.IgnoreCase);

        #endregion


        /// <summary>
        /// 过滤html代码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FiltHtmlCode(string str)
        {
            //string regexstr = @"<[^>]*>"; //filter html
            //return Regex.Replace(str, regexstr, string.Empty, RegexOptions.IgnoreCase);
            return regexHtmlCode.Replace(str, string.Empty);
        }

        /// <summary>
        /// 过滤UBB代码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FiltUBBCode(string str)
        {
            //string regexstr = @"\[(?<x>[^\]]*)\]"; //filter ubb
            //return Regex.Replace(str, regexstr, string.Empty, RegexOptions.IgnoreCase);
            return regexUbbCode.Replace(str, string.Empty);
        }

        /// <summary>
        /// 过滤HTML代码，过滤UBB代码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FiltCode(string str)
        {
            return FiltUBBCode(FiltHtmlCode(str));
        }

        /// <summary>
        /// 过滤脚本
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string FilterScript(string content)
        {
            if (content == null || content == "")
            {
                return content;
            }
            //string regexstr = @"(?i)<script([^>])*>(\w|\W)*</script([^>])*>";//@"<script.*</script>";
            //content = Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, "<script([^>])*>", string.Empty, RegexOptions.IgnoreCase);
            //return Regex.Replace(content, "</script>", string.Empty, RegexOptions.IgnoreCase);
            content = regexScript.Replace(content, string.Empty);
            content = regexScriptHeader.Replace(content, string.Empty);
            content = regexScriptFooter.Replace(content, string.Empty);
            return content;
        }

        /// <summary>
        /// 过滤iframe
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string FilterIFrame(string content)
        {
            if (content == null || content == "")
            {
                return content;
            }
            //string regexstr = @"(?i)<iframe([^>])*>(\w|\W)*</iframe([^>])*>";//@"<script.*</script>";
            //content = Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
            //content = Regex.Replace(content, "<iframe([^>])*>", string.Empty, RegexOptions.IgnoreCase);
            //return Regex.Replace(content, "</iframe>", string.Empty, RegexOptions.IgnoreCase);
            content = regexIFrame.Replace(content, string.Empty);
            content = regexIframeHeader.Replace(content, string.Empty);
            content = regexIframeFooter.Replace(content, string.Empty);
            return content;
        }

        /// <summary>
        /// 移除HTML标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            string newstr = FilterScript(content);
            //string regexstr = @"<[^>]*>";
            //return Regex.Replace(newstr, regexstr, string.Empty, RegexOptions.IgnoreCase);
            return regexAngularBracket.Replace(content, string.Empty);
        }

        /// <summary>
        /// 移除指定的HTML标签
        /// </summary>
        /// <param name="content"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string RemoveHtmlTag(string content, string[] tags)
        {
            string regexstr1, regexstr2;
            foreach (string tag in tags)
            {
                if (tag != "")
                {
                    regexstr1 = string.Format(@"<{0}([^>])*>", tag);
                    regexstr2 = string.Format(@"</{0}([^>])*>", tag);
                    content = Regex.Replace(content, regexstr1, string.Empty, RegexOptions.IgnoreCase);
                    content = Regex.Replace(content, regexstr2, string.Empty, RegexOptions.IgnoreCase);
                }
            }
            return content;

        }

        /// <summary>
        /// 移除一类HTML标签
        /// </summary>
        /// <param name="content"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string RemoveHtmlTag(string content, string tag)
        {
            string returnStr;
            string regexstr1 = string.Format(@"<{0}([^>])*>", tag);
            string regexstr2 = string.Format(@"</{0}([^>])*>", tag);
            returnStr = Regex.Replace(content, regexstr1, string.Empty, RegexOptions.IgnoreCase);
            returnStr = Regex.Replace(returnStr, regexstr2, string.Empty, RegexOptions.IgnoreCase);
            return returnStr;

        }

        /// <summary>
        /// 返回安全的文字格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SafeFormat(string str)
        {
            str = HttpContext.Current.Server.HtmlEncode(str);
            return str.Replace("\n", "<br>");
        }

        /// <summary>
        /// 返回安全的带有url的文字格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SafeFormatWithUrl(string str)
        {
            return EnableUrls(SafeFormat(str));
        }

        /// <summary>
        /// three-m 简化替换，直接排除前面有' "情况的链接
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EnableUrls(string text)
        {
            //text = Regex.Replace(text, @"(^|<br>|<br/>|[^('|\>|""|=|&quot;|&gt;)])((?:http|ftp|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?)", "$1<a href=\"$2\" target=\"_blank\" class=\"gray\">$2</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            text = regexUrl.Replace(text, "$1<a href=\"$2\" target=\"_blank\" class=\"gray\">$2</a>");
            return text;
        }

        /// <summary>
        /// 替换空格注释？
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ReplaceSpace(string content)
        {
            //string findstr = "(?<fore>(?:(?:[^< ])*(?:<(?:!--(?:(?:[^-])*(?:(?=-->)|-))*--|(?:[^>])+)>)?)*)[ ](?<back>(?:(?:[^< ])*(?:<(?:!--(?:(?:[^-])*(?:(?=-->)|-))*--|(?:[^>])+)>)?)*)";
            //"(?<fore>(?:[^< ]*(?:<[^>]+>)?)*)[ ](?<back>(?:[^< ]*(?:<[^>]+>)?)*)";
            string replacestr = "${fore}&nbsp;${back}";
            //string targetstr = System.Text.RegularExpressions.Regex.Replace(content, findstr, replacestr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string targetstr = regexSpace.Replace(content, replacestr);
            return targetstr;

        }

        /// <summary>
        /// 获取某一标签的所有HTML内容
        /// </summary>
        /// <param name="content"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string[] CatchHtmlBlock(string content, string tag)
        {
            string findstr = string.Format(@"(?i)<{0}([^>])*>(\w|\W)*</{1}([^>])*>", tag, tag);
            System.Text.RegularExpressions.MatchCollection matchs = System.Text.RegularExpressions.Regex.Matches(content, findstr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string[] strArray = new string[matchs.Count];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = matchs[i].Value;
            }
            return strArray;
        }

        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNumeric(string text)
        {
            //return Regex.IsMatch(text, "^\\d+$");
            return regexNumber.IsMatch(text);
        }

        /// <summary>
        /// 按字符串的实际长度截取定长字符串
        /// </summary>
        /// <param name="origStr">指定需要截取的字符串。</param>
        /// <param name="endIndex">指定需要截取的长度。</param>
        /// <returns></returns>
        public static string CutString(string origStr, int endIndex)
        {
            if (origStr != "")
            {
                int i = 0, j = 0;
                foreach (char Char in origStr)
                {
                    if ((int)Char > 127)
                        i += 2;
                    else
                        i++;
                    if (i > endIndex)
                    {
                        origStr = origStr.Substring(0, j) + "...";
                        break;
                    }
                    j++;
                }
                return origStr;

                /*byte[] bytes=System.Text.Encoding.GetEncoding("gb2312").GetBytes(origStr);
                //如果endIndex大于或等当前字符串的字节数，则返回整个字符串
                if(endIndex>=bytes.Length)
                    return origStr; 
                byte[] subBytes=new byte[endIndex];
                Array.Copy(bytes,0,subBytes,0,endIndex);
                return System.Text.Encoding.GetEncoding("gb2312").GetString(subBytes);*/
            }
            return "";
        }

        /// <summary>
        /// 简单UBB翻译器
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SimpleUbb(string str)
        {
            str = str.Replace("  ", " &nbsp;"); //替换两个英文空格为一个"&nbsp;"和一个英文空格
            str = str.Replace("\r\n", "<br/>");

            //[b][i]标记
            str = regexUbbB.Replace(str, @"<b>$1</b>");
            str = regexUbbB.Replace(str, @"<i>$1</i>");
            str = regexUbbB.Replace(str, @"<u>$1</u>");

            //[img]picname[/img]标记
            str = regexUbbImg.Replace(str, @"<a href=""$1"" target=_blank><img src=""$1"" border=0 alt='点击查看大图' onload='javascript:if(this.width>screen.width-450)this.width=screen.width-450'></a>");
            str = regexUbbImg2.Replace(str, "<a href=\"$1\" target=_blank><img src=\"$4\" border=0></a>");

            //[url][/url]标记
            str = regexUbbUrl.Replace(str, @"<a href=""$1"" target=_blank>$1</a>");
            //[url=xxx][/url]标记
            str = regexUbbUrl2.Replace(str, @"<a href=""$1"" target=_blank>$4</a>");

            str = regexAlign.Replace(str, @"<div align=$1>$2</div>");
            //[color=x][/color]标记
            str = regexColor.Replace(str, @"<font color=$1>$2</font>");

            //处[face=xxx][/face]标记
            str = regexFace.Replace(str, @"<font face=""$1"">$2</font>");

            //[flv]语音回复支持
            //str = Regex.Replace(str,@"\[flv\](.*?)\[/flv\]",@"<script language=""Javascript"">var vars='file=$1';GetFlash('flv_user_124', '/media/FlvPlayer.swf', vars, 149,27, false);</script>",RegexOptions.IgnoreCase);

            str = regexFlv.Replace(str, @"<div onclick=""ShowRecPlayer(this, '$1');"" class=""btnRecord""><img src=""/images/btn_RecPlayer.gif""/></div>");

            str = regexMp3.Replace(str, @"<div onclick=""ShowMp3Player(this, '$1');"" class=""btnRecord""><img src=""/images/btn_RecPlayer.gif""/></div>");

            //[iframe][/iframe]iframe引用
            str = regexIframeWH.Replace(str, @"<iframe width=""$1"" height=""$2"" border=""0"" scrolling=""auto"" frameborder=""0"" src=""$3"" style=""overflow:hidden""></iframe>");

            //[vote][/vote]调查通引用
            str = regexVote.Replace(str, @"<iframe width=""$1"" height=""$2"" border=""0"" scrolling=""auto"" frameborder=""0"" src=""http://vote.yeshj.com/$3"" style=""overflow:hidden""></iframe>");

            str = regexHJPlay1.Replace(str,
                "<script src='http://bulo.hjenglish.com/podcast/hjplayer.aspx?width=$1&height=$2&src=$5&autoplay=$3'></script>");

            str = regexHJPlay2.Replace(str,
                "<script src='http://www.hjenglish.com/common/getmediawindow.js' type='text/javascript'></script><script language='JavaScript'>GetMediaPlayerWindow( '$5', $1, $2);</script>");

            str = regexHJPlay3.Replace(str,
               @"<div onclick=""ShowMp3Player(this, '$1');"" class=""btnRecord""><img src=""/images/btn_RecPlayer.gif""/></div>");


            str = regexHJPlay32.Replace(str,
                "<script src='http://bulo.hjenglish.com/musicbox/listcodemaker.aspx?$1' type='text/javascript'></script>");

            //[quote][/quote]引用回复
            str = regexQuote.Replace(str, @"<div class=""brief_quotes""><q>$1</q></div>");

            //[replyview][/replyview]引用回复
            str = regexReplyView.Replace(str, @"<div class=""replyview"">------------------<br/><span class=red>回复可见内容</span><br/>------------------</div>");
            //[w]小D支持
            str = regexD.Replace(str, @"<a href=""http://dict.hjenglish.com/w/$1"" class=""hjdict"" word=""$1"" target=_blank>$1</a>");
            str = regexD2.Replace(str, @"<a href=""http://dict.hjenglish.com/w/$1""  class=""hjdict"" word=""$1"" target=_blank>$2</a>");
            str = regexD3.Replace(str, @"<a href=""http://dict.hjenglish.com/w/$1"" class=""hjdict"" word=""$1"" target=_blank>$1</a><script language=""Javascript"">GetWord(""$1"");</script>");

            //[w]小D日文支持
            str = regexDJ.Replace(str, @"<a href=""http://dict.hjenglish.com/jp/w/$1"" class=""hjdict"" word=""$1"" target=_blank>$1</a>");
            str = regexDJ2.Replace(str, @"<a href=""http://dict.hjenglish.com/jp/w/$1&type=$2"" class=""hjdict"" word=""$1&type=$2"" target=_blank>$3</a>");
            str = regexDJ3.Replace(str, @"<a href=""http://dict.hjenglish.com/jp/w/$1&type=jc"" class=""hjdict"" word=""$1"" target=_blank>$2</a><script language=""Javascript"">GetWord(""jp_$1"");</script>");



            /*
            表达式：[flash=500,400]url[/flash]
            说明：    支持flash
            参数：    width为flash宽度
                    height为flash高度
                    url为flash的地址    */
            str = regexFlash.Replace(str,
                "<OBJECT codeBase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,28,0\" "
                + " classid=\"clsid:d27CDB6E-AE6D-11cf-96B8-444553540000\" "
                + "width=$1 height=$2>"
                + "<PARAM NAME=movie VALUE=\"$3\">"
                + "<PARAM NAME=quality VALUE=high>"
                + "<embed src=\"$3\" width=$1 height=$2 quality=high name=index type=\"application/x-shockwave-flash\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\"/>"
                + "</OBJECT>"
                );

            //[ipa]标记
            //注意iPA音标标记要放在最后替换
            //因为ipa会 [ipa]文本[/ipa] -> <span class='ipa'>[文本]</span>
            //这样添加的[,]标记会影响其他的ubb标签匹配
            str = regexIPA.Replace(str, "<span class='IPA big'>[$1]</span>");

            str = EnableUrls(str);

            return str;

        }

        /// <summary>
        /// 对于输入是字符类型的检查
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetParamString(object obj)
        {
            if (obj == null || obj.ToString() == string.Empty) return String.Empty;
            string objString = String.Empty;
            objString = HttpContext.Current.Server.HtmlEncode(objString);
            objString = obj.ToString().Replace("'", "");
            objString = objString.Replace("--", "");
            //objString = objString.Replace(" ",""); //过滤参数中的空格
            return objString;
        }

        /// <summary>
        /// Global.asax检查 记录SQL注入企图
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public static bool CheckSQLInjection(string strSQL)
        {
            if (string.IsNullOrEmpty(strSQL))
                return true;
            else
            {
                //Regex RegExpression = new Regex(@"\s");
                strSQL = regexBlank.Replace(strSQL.Trim().Trim().ToLower().Replace("%20", " "), " ");
                //string Pattern = @"select |insert |delete from |count\(|drop table|update |truncate |asc\(|mid\(|char\(|xp_cmdshell|exec master|net localgroup administrators|:|net user|""|\'| or ";
                return regexSql.IsMatch(strSQL);
            }
        }

        //2009/8/11 by windir1181
        /// <summary>
        /// 功能：统一符号到半角逗号并且格式化成aaa,bbb,ccc形式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceComma(string input)
        {
            input = input.Replace("，", "|");
            input = input.Replace(",", "|");
            input = input.Replace(" ", "|");
            if (input.Trim().EndsWith("|"))
            {
                input = input.Substring(0, input.LastIndexOf('|'));
            }
            return input;
        }

        /// <summary>
        /// 功能：由竖线格式化成半角逗号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceVertical(string input)
        {
            input = input.Replace("|", ",");
            if (input.EndsWith(","))
            {
                input = input.Substring(0, input.LastIndexOf(","));
            }
            return input;
        }

        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        public static string GetRandSN(int strLength)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(36);
            Random rd = new Random();
            for (int i = 0; i < strLength; i++)
            {
                newRandom.Append(constant[rd.Next(36)]);
            }
            return newRandom.ToString();
        }
    }
}
