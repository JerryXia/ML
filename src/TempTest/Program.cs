using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using System.IO;

using ML.Net.Http;
using System.Net;
using System.Drawing;


namespace TempTest
{
    class Program
    {

        static void Main(string[] args)
        {
            #region 1

            //Console.WriteLine(Global.AppWebDir);
            //Console.WriteLine(Global.App_DataDir);

            //string xmlFileName = "CustomXmlConfig.xml";

            //CustomXmlConfig cusXmlConfig = new CustomXmlConfig(xmlFileName);

            //TestRunOption testRunOption1 = new TestRunOption { Name = "JerryXia", Age = 22 };
            //XmlSerialization.XmlSerializeToFile(testRunOption1, Path.Combine(Global.App_DataDir, xmlFileName), Encoding.UTF8);
            //IRunOptions testRunOption2 = cusXmlConfig.LoadRunOptions();
            //Console.WriteLine(testRunOption2.Age);
            //Console.WriteLine(testRunOption2.Name);
            //while(true)
            //{
                
            //}

            //CodeTimer.Time("Thread Sleep", 1, () => { Thread.Sleep(3000); });
            //CodeTimer.Time("Empty Method", 10000000, () => { });

            //int iteration = 100 * 1000;

            //string s = "";
            //CodeTimer.Time("String Concat", iteration, () => { s += "a"; });

            //StringBuilder sb = new StringBuilder();
            //CodeTimer.Time("StringBuilder", iteration, () => { sb.Append("a"); });

            #endregion


            //首次访问
            string url = "https://sh.ac.10086.cn/login";
            HttpRequestWrapper httpWraper = new HttpRequestWrapper(url, HttpRequestMethod.GET, Encoding.UTF8);
            httpWraper.Request();

            Console.WriteLine("请输入您的手机号：");
            string mobile = Console.ReadLine();
            Console.WriteLine("您的手机号是：" + mobile);

            Console.WriteLine("请输入您的服务密码：");
            string pwd = Console.ReadLine();
            Console.WriteLine("您的服务密码是：" + pwd);


            //获取图片验证码  https://sh.ac.10086.cn/validationCode?rnd=506 
            string imgValidateCodeUrl = "https://sh.ac.10086.cn/validationCode?rnd=" + new Random().Next(1, 1000);
            httpWraper.RequestSetting.ResultDataType = HttpResponseDataType.Byte;
            httpWraper.Request(imgValidateCodeUrl, HttpRequestMethod.GET, Encoding.UTF8);

            //识别
            //Stream stream = new MemoryStream(httpWraper.HttpResult.ByteResult);
            //Image img = Bitmap.FromStream(stream);
            //unCodeAiYing UnCheckobj = new unCodeAiYing(new Bitmap(img));
            //string strNum = UnCheckobj.getPicnum();

            Console.WriteLine("请输入验证码：");
            string strNum = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(strNum))
            {
                //模拟登录
                string postUrl = string.Format("https://sh.ac.10086.cn/loginex?act=2&telno={0}&password={1}&authLevel=2&validcode={2}&ctype=1&t={3}", mobile, pwd, strNum, GetTimeMiliMinutes(DateTime.Now));
                httpWraper.RequestSetting.ResultDataType = HttpResponseDataType.String;
                httpWraper.Request(postUrl, HttpRequestMethod.POST, Encoding.UTF8);
                Console.WriteLine("登录返回结果：" + httpWraper.HttpResult.Html);

                var step1CbObj = Newtonsoft.Json.JsonConvert.DeserializeObject<SsoLoginCallback>(httpWraper.HttpResult.Html);
                if (!string.IsNullOrWhiteSpace(step1CbObj.uid))
                {
                    string step1CbUrl = string.Format("http://www.sh.10086.cn/sh/wsyyt/ac/forward.jsp?uid={0}&tourl=http://www.sh.10086.cn/sh/service/&t={1}", step1CbObj.uid, GetTimeMiliMinutes(DateTime.Now));
                    httpWraper.Request(step1CbUrl, HttpRequestMethod.GET, Encoding.UTF8);
                    Console.WriteLine("step1返回结果：" + httpWraper.HttpResult.Html);

                    string step2Url = "http://www.sh.10086.cn/sh/service/";
                    httpWraper.Request(step2Url, HttpRequestMethod.GET, Encoding.UTF8);


                    //发送短信
                    string sendMsgUrl = string.Format("https://sh.ac.10086.cn/loginex?iscb=1&act=1&telno={0}&t={1}", mobile, GetTimeMiliMinutes(DateTime.Now));
                    httpWraper.Request(sendMsgUrl, HttpRequestMethod.GET, Encoding.UTF8);
                    Console.WriteLine("发送短信结果：" + httpWraper.HttpResult.Html);


                    Console.WriteLine("请输入验证码");
                    string input = Console.ReadLine();
                    //验证验证码
                    string code = input;
                    Console.WriteLine("您输入的验证码是：" + code);
                    string checkCodeUrl = string.Format("https://sh.ac.10086.cn/loginex?iscb=1&act=2&telno={0}&password={1}&authLevel=1&validcode=&t={2}", mobile, code, GetTimeMiliMinutes(DateTime.Now));
                    httpWraper.Request(checkCodeUrl, HttpRequestMethod.GET, Encoding.UTF8);
                    Console.WriteLine("验证验证码结果：" + httpWraper.HttpResult.Html);

                    // 返回结果
                    // ssoLoginCallback({"brand":"2","result":"0","uid":"9d08b18b47a24595b22feb496214a8bb","message":""});
                    Regex regex = new Regex(@"ssoLoginCallback\((.+)\)", RegexOptions.Singleline);
                    Match match = regex.Match(httpWraper.HttpResult.Html);
                    if (match.Success)
                    {
                        var ssoLoginCallback = Newtonsoft.Json.JsonConvert.DeserializeObject<SsoLoginCallback>(match.Groups[1].Value);

                        string smsCheckCallbackUrl = "http://www.sh.10086.cn/sh/wsyyt/busi.json?sid=WF000022";
                        httpWraper.RequestSetting.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        httpWraper.RequestSetting.PostStringData = "uid=" + ssoLoginCallback.uid;
                        httpWraper.Request(smsCheckCallbackUrl, HttpRequestMethod.POST, Encoding.UTF8);
                        Console.WriteLine("验证码回调验证结果：" + httpWraper.HttpResult.Html);

                        // 获取历史详单
                        string getFiveBillDetailAjaxUrl = "http://www.sh.10086.cn/sh/wsyyt/busi/historySearch.do?method=getFiveBillDetailAjax";
                        httpWraper.RequestSetting.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        httpWraper.RequestSetting.PostStringData = "billType=NEW_GSM&startDate=2014-09-01&endDate=2014-09-30&filterfield=输入对方号码：&filterValue=&searchStr=-1&index=0&r=1413705013428&isCardNo=0&gprsType=";
                        httpWraper.Request(getFiveBillDetailAjaxUrl, HttpRequestMethod.POST, Encoding.UTF8);
                        Console.WriteLine("详单返回结果：" + httpWraper.HttpResult.Html);
                    }
                }
                else
                {
                    //Console.WriteLine();
                }
            }

            Console.ReadKey();
        }

        static double GetTimeMiliMinutes(DateTime time)
        {
            DateTime origin = new DateTime(1970, 1, 1);
            return Convert.ToInt64((time - origin).TotalMilliseconds);
        }

    }

    class SsoLoginCallback
    {
        public string brand {get;set;}
        public string result {get;set;}
        public string uid {get;set;}
        public string message{get;set;}
    }

    public class HttpRequestWrapper
    {
        HttpRequestSetting reqSetting = null;
        HttpRequest httpRequest = null;
        HttpResponse httpResponse = null;

        CookieParser cookieParser = null;
        CookieCollection curCookies = null;

        public HttpRequestWrapper(string url, HttpRequestMethod method, Encoding encode)
        {
            reqSetting = new HttpRequestSetting(url, method, encode);
            httpRequest = new HttpRequest();

            cookieParser = new CookieParser();
        }

        public void Request()
        {
            reqSetting.CookieCollection = curCookies;
            httpResponse = httpRequest.GetResponse(reqSetting);

            int statusCode = (int)httpResponse.StatusCode;
            switch (statusCode)
            {
                case 301:
                case 302:
                    {
                        string curDomain = cookieParser.ExtractDomain(reqSetting.Url);
                        foreach (string key in httpResponse.Headers.AllKeys)
                        {
                            var cookieColl = cookieParser.ParseSetCookie(httpResponse.Headers.Get(key), curDomain);
                            cookieParser.UpdateLocalCookies(cookieColl, ref curCookies);
                        }

                        reqSetting.Url = httpResponse.Headers.Get("Location");
                        reqSetting.CookieCollection = curCookies;
                        Request();
                    }
                    break;
                case 200:
                default:
                    {
                        string curDomain = cookieParser.ExtractDomain(reqSetting.Url);
                        foreach (string key in httpResponse.Headers.AllKeys)
                        {
                            var cookieColl = cookieParser.ParseSetCookie(httpResponse.Headers.Get(key), curDomain);
                            cookieParser.UpdateLocalCookies(cookieColl, ref curCookies);
                        }
                    }
                    break;
            }
        }

        public void Request(string url, HttpRequestMethod method, Encoding encode)
        {
            reqSetting.Url = url;
            reqSetting.Method = method;
            reqSetting.Encoding = encode;

            Request();
        }

        public HttpResponse HttpResult
        {
            get { return httpResponse; }
        }

        public HttpRequestSetting RequestSetting
        {
            get { return reqSetting; }
            set { reqSetting = value; }
        }

    }


}
