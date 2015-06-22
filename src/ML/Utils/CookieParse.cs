using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web; // for server
using System.Net; // for client
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace ML.Utils
{
    public class CookieParse
    {

        public struct pairItem
        {
            public string key;
            public string value;
        };

        private Dictionary<string, DateTime> calcTimeList;

        const char replacedChar = '_';

        string[] cookieFieldArr = { "expires", "domain", "secure", "path", "httponly", "version" };
        List<string> cookieFieldList = new List<string>();

        CookieCollection curCookies = null;

        public CookieParse()
        {
            //http related
            //set max enough to avoid http request is used out -> avoid dead while get response
            System.Net.ServicePointManager.DefaultConnectionLimit = 200;

            curCookies = new CookieCollection();

            // init const cookie keys
            foreach (string key in cookieFieldArr)
            {
                cookieFieldList.Add(key);
            }

            //init for calc time
            calcTimeList = new Dictionary<string, DateTime>();
        }

        /*------------------------Private Functions------------------------------*/

        // replace the replacedChar back to original ','
        private string _recoverExpireField(Match foundPprocessedExpire)
        {
            string recovedStr = "";
            recovedStr = foundPprocessedExpire.Value.Replace(replacedChar, ',');
            return recovedStr;
        }

        //replace ',' with replacedChar
        private string _processExpireField(Match foundExpire)
        {
            string replacedComma = "";
            replacedComma = foundExpire.Value.ToString().Replace(',', replacedChar);
            return replacedComma;
        }


        /*------------------------Public Functions-------------------------------*/

        /*********************************************************************/
        /* Values: int/double/... */
        /*********************************************************************/

        //equivalent of Math.Random() in Javascript
        //get a 17 bit double value x, 0 < x < 1, eg:0.68637410117610087
        public double mathRandom()
        {
            Random rdm = new Random();
            double betweenZeroToOne17Bit = rdm.NextDouble();
            return betweenZeroToOne17Bit;
        }

        /*********************************************************************/
        /* Time */
        /*********************************************************************/

        // init for calculate time span
        public void elapsedTimeSpanInit(string keyName)
        {
            calcTimeList.Add(keyName, DateTime.Now);
        }

        // got calculated time span
        public double getElapsedTimeSpan(string keyName)
        {
            double milliSec = 0.0;
            if (calcTimeList.ContainsKey(keyName))
            {
                DateTime startTime = calcTimeList[keyName];
                DateTime endTime = DateTime.Now;
                milliSec = (endTime - startTime).TotalMilliseconds;
            }
            return milliSec;
        }

        //refer: http://bytes.com/topic/c-sharp/answers/713458-c-function-equivalent-javascript-gettime-function
        //get current time in milli-second-since-epoch(1970/01/01)
        public double getCurTimeInMillisec()
        {
            DateTime st = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.Now - st);
            return t.TotalMilliseconds; // milli seconds since epoch
        }

        // parse the milli second to local DateTime value
        public DateTime milliSecToDateTime(double milliSecSinceEpoch)
        {
            DateTime st = new DateTime(1970, 1, 1, 0, 0, 0);
            st = st.AddMilliseconds(milliSecSinceEpoch);
            return st;
        }

        /*********************************************************************/
        /* String */
        /*********************************************************************/

        // encode "!" to "%21"
        public string encodeExclamationMark(string inputStr)
        {
            return inputStr.Replace("!", "%21");
        }

        // encode "%21" to "!"
        public string decodeExclamationMark(string inputStr)
        {
            return inputStr.Replace("%21", "!");
        }

        //using Regex to extract single string value
        // caller should make sure the string to extract is Groups[1] == include single () !!!
        public bool extractSingleStr(string pattern, string extractFrom, out string extractedStr)
        {
            bool extractOK = false;
            Regex rx = new Regex(pattern);
            Match found = rx.Match(extractFrom);
            if (found.Success)
            {
                extractOK = true;
                extractedStr = found.Groups[1].ToString();
            }
            else
            {
                extractOK = false;
                extractedStr = "";
            }

            return extractOK;
        }

        //quote the input dict values
        //note: the return result for first para no '&'
        public string quoteParas(Dictionary<string, string> paras)
        {
            string quotedParas = "";
            bool isFirst = true;
            string val = "";
            foreach (string para in paras.Keys)
            {
                if (paras.TryGetValue(para, out val))
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        quotedParas += para + "=" + HttpUtility.UrlPathEncode(val);
                    }
                    else
                    {
                        quotedParas += "&" + para + "=" + HttpUtility.UrlPathEncode(val);
                    }
                }
                else
                {
                    break;
                }
            }

            return quotedParas;
        }

        //remove invalid char in path and filename
        public string removeInvChrInPath(string origFileOrPathStr)
        {
            string validFileOrPathStr = origFileOrPathStr;

            //filter out invalid title and artist char
            //char[] invalidChars = { '\\', '/', ':', '*', '?', '<', '>', '|', '\b' };
            char[] invalidChars = Path.GetInvalidPathChars();
            char[] invalidCharsInName = Path.GetInvalidFileNameChars();

            foreach (char chr in invalidChars)
            {
                validFileOrPathStr = validFileOrPathStr.Replace(chr.ToString(), "");
            }

            foreach (char chr in invalidCharsInName)
            {
                validFileOrPathStr = validFileOrPathStr.Replace(chr.ToString(), "");
            }

            return validFileOrPathStr;
        }

        /*********************************************************************/
        /* Array */
        /*********************************************************************/

        //given a string array 'origStrArr', get a sub string array from 'startIdx', length is 'len'
        public string[] getSubStrArr(string[] origStrArr, int startIdx, int len)
        {
            string[] subStrArr = new string[] { };
            if ((origStrArr != null) && (origStrArr.Length > 0) && (len > 0))
            {
                List<string> strList = new List<string>();
                int endPos = startIdx + len;
                if (endPos > origStrArr.Length)
                {
                    endPos = origStrArr.Length;
                }

                for (int i = startIdx; i < endPos; i++)
                {
                    //refer: http://zhidao.baidu.com/question/296384408.html
                    strList.Add(origStrArr[i]);
                }

                subStrArr = new string[len];
                strList.CopyTo(subStrArr);
            }

            return subStrArr;
        }

        /*********************************************************************/
        /* cookie */
        /*********************************************************************/

        //extrat the Host from input url
        //example: from https://skydrive.live.com/, extracted Host is "skydrive.live.com"
        public string extractHost(string url)
        {
            string domain = "";
            if ((url != "") && (url.Contains("/")))
            {
                string[] splited = url.Split('/');
                domain = splited[2];
            }
            return domain;
        }

        //extrat the domain from input url
        //example: from https://skydrive.live.com/, extracted domain is ".live.com"
        public string extractDomain(string url)
        {
            string host = "";
            string domain = "";
            host = extractHost(url);
            if (host.Contains("."))
            {
                domain = host.Substring(host.IndexOf('.'));
            }
            return domain;
        }

        //add recognized cookie field: expires/domain/path/secure/httponly/version, into cookie
        public bool addFieldToCookie(ref Cookie ck, pairItem pairInfo)
        {
            bool added = false;
            if (pairInfo.key != "")
            {
                string lowerKey = pairInfo.key.ToLower();
                switch (lowerKey)
                {
                    case "expires":
                        DateTime expireDatetime;
                        if (DateTime.TryParse(pairInfo.value, out expireDatetime))
                        {
                            // note: here coverted to local time: GMT +8
                            ck.Expires = expireDatetime;

                            //update expired filed
                            if (DateTime.Now.Ticks > ck.Expires.Ticks)
                            {
                                ck.Expired = true;
                            }

                            added = true;
                        }
                        break;
                    case "domain":
                        ck.Domain = pairInfo.value;
                        added = true;
                        break;
                    case "secure":
                        ck.Secure = true;
                        added = true;
                        break;
                    case "path":
                        ck.Path = pairInfo.value;
                        added = true;
                        break;
                    case "httponly":
                        ck.HttpOnly = true;
                        added = true;
                        break;
                    case "version":
                        int versionValue;
                        if (int.TryParse(pairInfo.value, out versionValue))
                        {
                            ck.Version = versionValue;
                            added = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            return added;
        }//addFieldToCookie

        public bool isValidCookieField(string cookieKey)
        {
            return cookieFieldList.Contains(cookieKey.ToLower());
        }

        //cookie field example:
        //WLSRDAuth=FAAaARQL3KgEDBNbW84gMYrDN0fBab7xkQNmAAAEgAAACN7OQIVEO14E2ADnX8vEiz8fTuV7bRXem4Yeg/DI6wTk5vXZbi2SEOHjt%2BbfDJMZGybHQm4NADcA9Qj/tBZOJ/ASo5d9w3c1bTlU1jKzcm2wecJ5JMJvdmTCj4J0oy1oyxbMPzTc0iVhmDoyClU1dgaaVQ15oF6LTQZBrA0EXdBxq6Mu%2BUgYYB9DJDkSM/yFBXb2bXRTRgNJ1lruDtyWe%2Bm21bzKWS/zFtTQEE56bIvn5ITesFu4U8XaFkCP/FYLiHj6gpHW2j0t%2BvvxWUKt3jAnWY1Tt6sXhuSx6CFVDH4EYEEUALuqyxbQo2ugNwDkP9V5O%2B5FAyCf; path=/; domain=.livefilestore.com;  HttpOnly;,
        //WLSRDSecAuth=FAAaARQL3KgEDBNbW84gMYrDN0fBab7xkQNmAAAEgAAACJFcaqD2IuX42ACdjP23wgEz1qyyxDz0kC15HBQRXH6KrXszRGFjDyUmrC91Zz%2BgXPFhyTzOCgQNBVfvpfCPtSccxJHDIxy47Hq8Cr6RGUeXSpipLSIFHumjX5%2BvcJWkqxDEczrmBsdGnUcbz4zZ8kP2ELwAKSvUteey9iHytzZ5Ko12G72%2Bbk3BXYdnNJi8Nccr0we97N78V0bfehKnUoDI%2BK310KIZq9J35DgfNdkl12oYX5LMIBzdiTLwN1%2Bx9DgsYmmgxPbcuZPe/7y7dlb00jNNd8p/rKtG4KLLT4w3EZkUAOcUwGF746qfzngDlOvXWVvZjGzA; path=/; domain=.livefilestore.com;  HttpOnly; secure;,
        //RPSShare=1; path=/;,
        //ANON=A=DE389D4D076BF47BCAE4DC05FFFFFFFF&E=c44&W=1; path=/; domain=.livefilestore.com;,
        //NAP=V=1.9&E=bea&C=VTwb1vAsVjCeLWrDuow-jCNgP5eS75JWWvYVe3tRppviqKixCvjqgw&W=1; path=/; domain=.livefilestore.com;,
        //RPSMaybe=; path=/; domain=.livefilestore.com; expires=Thu, 30-Oct-1980 16:00:00 GMT;

        //check whether the cookie name is valid or not
        public bool isValidCookieName(string ckName)
        {
            bool isValid = true;
            if (ckName == null)
            {
                isValid = false;
            }
            else
            {
                //string invalidP = @"\W+";

                //Regex rx = new Regex(invalidP);
                //Match foundInvalid = rx.Match(ckName);
                //if (foundInvalid.Success)
                //{
                //    isValid = false;
                //}
                string invalidP = @".+";
                Regex rx = new Regex(invalidP);
                Match foundInvalid = rx.Match(ckName);
                if (!foundInvalid.Success)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        // parse the cookie name and value
        public bool parseCookieNameValue(string ckNameValueExpr, out pairItem pair)
        {
            bool parsedOK = false;
            if (ckNameValueExpr == "")
            {
                pair.key = "";
                pair.value = "";
                parsedOK = false;
            }
            else
            {
                ckNameValueExpr = ckNameValueExpr.Trim();

                int equalPos = ckNameValueExpr.IndexOf('=');
                if (equalPos > 0) // is valid expression
                {
                    pair.key = ckNameValueExpr.Substring(0, equalPos);
                    pair.key = pair.key.Trim();
                    if (isValidCookieName(pair.key))
                    {
                        // only process while is valid cookie field
                        pair.value = ckNameValueExpr.Substring(equalPos + 1);
                        pair.value = pair.value.Trim();
                        parsedOK = true;
                    }
                    else
                    {
                        pair.key = "";
                        pair.value = "";
                        parsedOK = false;
                    }
                }
                else
                {
                    pair.key = "";
                    pair.value = "";
                    parsedOK = false;
                }
            }
            return parsedOK;
        }

        // parse cookie field expression
        public bool parseCookieField(string ckFieldExpr, out pairItem pair)
        {
            bool parsedOK = false;

            if (ckFieldExpr == "")
            {
                pair.key = "";
                pair.value = "";
                parsedOK = false;
            }
            else
            {
                ckFieldExpr = ckFieldExpr.Trim();

                //some specials: secure/httponly
                if (ckFieldExpr.ToLower() == "httponly")
                {
                    pair.key = "httponly";
                    //pair.value = "";
                    pair.value = "true";
                    parsedOK = true;
                }
                else if (ckFieldExpr.ToLower() == "secure")
                {
                    pair.key = "secure";
                    //pair.value = "";
                    pair.value = "true";
                    parsedOK = true;
                }
                else // normal cookie field
                {
                    int equalPos = ckFieldExpr.IndexOf('=');
                    if (equalPos > 0) // is valid expression
                    {
                        pair.key = ckFieldExpr.Substring(0, equalPos);
                        pair.key = pair.key.Trim();
                        if (isValidCookieField(pair.key))
                        {
                            // only process while is valid cookie field
                            pair.value = ckFieldExpr.Substring(equalPos + 1);
                            pair.value = pair.value.Trim();
                            parsedOK = true;
                        }
                        else
                        {
                            pair.key = "";
                            pair.value = "";
                            parsedOK = false;
                        }
                    }
                    else
                    {
                        pair.key = "";
                        pair.value = "";
                        parsedOK = false;
                    }
                }
            }

            return parsedOK;
        }//parseCookieField

        //parse single cookie string to a cookie
        //example:
        //MSPShared=1; expires=Wed, 30-Dec-2037 16:00:00 GMT;domain=login.live.com;path=/;HTTPOnly= ;version=1
        //PPAuth=CkLXJYvPpNs3w!fIwMOFcraoSIAVYX3K!CdvZwQNwg3Y7gv74iqm9MqReX8XkJqtCFeMA6GYCWMb9m7CoIw!ID5gx3pOt8sOx1U5qQPv6ceuyiJYwmS86IW*l3BEaiyVCqFvju9BMll7!FHQeQholDsi0xqzCHuW!Qm2mrEtQPCv!qF3Sh9tZDjKcDZDI9iMByXc6R*J!JG4eCEUHIvEaxTQtftb4oc5uGpM!YyWT!r5jXIRyxqzsCULtWz4lsWHKzwrNlBRbF!A7ZXqXygCT8ek6luk7rarwLLJ!qaq2BvS; domain=login.live.com;secure= ;path=/;HTTPOnly= ;version=1
        public bool parseSingleCookie(string cookieStr, ref Cookie ck)
        {
            bool parsedOk = true;
            //Cookie ck = new Cookie();
            //string[] expressions = cookieStr.Split(";".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
            //refer: http://msdn.microsoft.com/en-us/library/b873y76a.aspx
            string[] expressions = cookieStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //get cookie name and value
            pairItem pair = new pairItem();
            if (parseCookieNameValue(expressions[0], out pair))
            {
                ck.Name = pair.key;
                ck.Value = pair.value;

                string[] fieldExpressions = getSubStrArr(expressions, 1, expressions.Length - 1);
                foreach (string eachExpression in fieldExpressions)
                {
                    //parse key and value
                    if (parseCookieField(eachExpression, out pair))
                    {
                        // add to cookie field if possible
                        addFieldToCookie(ref ck, pair);
                    }
                    else
                    {
                        // if any field fail, consider it is a abnormal cookie string, so quit with false
                        parsedOk = false;
                        break;
                    }
                }
            }
            else
            {
                parsedOk = false;
            }

            return parsedOk;
        }//parseSingleCookie

        //check whether need add/retain this cookie
        // not add for:
        // ck is null or ck name is null
        // domain is null and curDomain is not set
        // expired and retainExpiredCookie==false
        private bool needAddThisCookie(Cookie ck, string curDomain)
        {
            bool needAdd = false;

            if ((ck == null) || (ck.Name == ""))
            {
                needAdd = false;
            }
            else
            {
                if (ck.Domain != "")
                {
                    needAdd = true;
                }
                else// ck.Domain == ""
                {
                    if (curDomain != "")
                    {
                        ck.Domain = curDomain;
                        needAdd = true;
                    }
                    else // curDomain == ""
                    {
                        // not set current domain, omit this
                        // should not add empty domain cookie, for this will lead execute CookieContainer.Add() fail !!!
                        needAdd = false;
                    }
                }
            }

            return needAdd;
        }

        // parse the Set-Cookie string (in http response header) to cookies
        // Note: auto omit to parse the abnormal cookie string
        // normal example for 'setCookieStr':
        // MSPOK= ; expires=Thu, 30-Oct-1980 16:00:00 GMT;domain=login.live.com;path=/;HTTPOnly= ;version=1,PPAuth=Cuyf3Vp2wolkjba!TOr*0v22UMYz36ReuiwxZZBc8umHJYPlRe4qupywVFFcIpbJyvYZ5ZDLBwV4zRM1UCjXC4tUwNuKvh21iz6gQb0Tu5K7Z62!TYGfowB9VQpGA8esZ7iCRucC7d5LiP3ZAv*j4Z3MOecaJwmPHx7!wDFdAMuQUZURhHuZWJiLzHP1j8ppchB2LExnlHO6IGAdZo1f0qzSWsZ2hq*yYP6sdy*FdTTKo336Q1B0i5q8jUg1Yv6c2FoBiNxhZSzxpuU0WrNHqSytutP2k4!wNc6eSnFDeouX; domain=login.live.com;secure= ;path=/;HTTPOnly= ;version=1,PPLState=1; domain=.live.com;path=/;version=1,MSPShared=1; expires=Wed, 30-Dec-2037 16:00:00 GMT;domain=login.live.com;path=/;HTTPOnly= ;version=1,MSPPre= ;domain=login.live.com;path=/;Expires=Thu, 30-Oct-1980 16:00:00 GMT,MSPCID= ; HTTPOnly= ; domain=login.live.com;path=/;Expires=Thu, 30-Oct-1980 16:00:00 GMT,RPSTAuth=EwDoARAnAAAUWkziSC7RbDJKS1VkhugDegv7L0eAAOfCAY2+pKwbV5zUlu3XmBbgrQ8EdakmdSqK9OIKfMzAbnU8fuwwEi+FKtdGSuz/FpCYutqiHWdftd0YF21US7+1bPxuLJ0MO+wVXB8GtjLKZaA0xCXlU5u01r+DOsxSVM777DmplaUc0Q4O1+Pi9gX9cyzQLAgRKmC/QtlbVNKDA2YAAAhIwqiXOVR/DDgBocoO/n0u48RFGh79X2Q+gO4Fl5GMc9Vtpa7SUJjZCCfoaitOmcxhEjlVmR/2ppdfJx3Ykek9OFzFd+ijtn7K629yrVFt3O9q5L0lWoxfDh5/daLK7lqJGKxn1KvOew0SHlOqxuuhYRW57ezFyicxkxSI3aLxYFiqHSu9pq+TlITqiflyfcAcw4MWpvHxm9on8Y1dM2R4X3sxuwrLQBpvNsG4oIaldTYIhMEnKhmxrP6ZswxzteNqIRvMEKsxiksBzQDDK/Cnm6QYBZNsPawc6aAedZioeYwaV3Z/i3tNrAUwYTqLXve8oG6ZNXL6WLT/irKq1EMilK6Cw8lT3G13WYdk/U9a6YZPJC8LdqR0vAHYpsu/xRF39/On+xDNPE4keIThJBptweOeWQfsMDwvgrYnMBKAMjpLZwE=; domain=.live.com;path=/;HTTPOnly= ;version=1,RPSTAuthTime=1328679636; domain=login.live.com;path=/;HTTPOnly= ;version=1,MSPAuth=2OlAAMHXtDIFOtpaK1afG2n*AAxdfCnCBlJFn*gCF8gLnCa1YgXEfyVh2m9nZuF*M7npEwb4a7Erpb*!nH5G285k7AswJOrsr*gY29AVAbsiz2UscjIGHkXiKrTvIzkV2M; domain=.live.com;path=/;HTTPOnly= ;version=1,MSPProf=23ci9sti6DZRrkDXfTt1b3lHhMdheWIcTZU2zdJS9!zCloHzMKwX30MfEAcCyOjVt*5WeFSK3l2ZahtEaK7HPFMm3INMs3r!JxI8odP9PYRHivop5ryohtMYzWZzj3gVVurcEr5Bg6eJJws7rXOggo3cR4FuKLtXwz*FVX0VWuB5*aJhRkCT1GZn*L5Pxzsm9X; domain=.live.com;path=/;HTTPOnly= ;version=1,MSNPPAuth=CiGSMoUOx4gej8yQkdFBvN!gvffvAhCPeWydcrAbcg!O2lrhVb4gruWSX5NZCBPsyrtZKmHLhRLTUUIxxPA7LIhqW5TCV*YcInlG2f5hBzwzHt!PORYbg79nCkvw65LKG399gRGtJ4wvXdNlhHNldkBK1jVXD4PoqO1Xzdcpv4sj68U6!oGrNK5KgRSMXXpLJmCeehUcsRW1NmInqQXpyanjykpYOcZy0vq!6PIxkj3gMaAvm!1vO58gXM9HX9dA0GloNmCDnRv4qWDV2XKqEKp!A7jiIMWTmHup1DZ!*YCtDX3nUVQ1zAYSMjHmmbMDxRJECz!1XEwm070w16Y40TzuKAJVugo!pyF!V2OaCsLjZ9tdGxGwEQRyi0oWc*Z7M0FBn8Fz0Dh4DhCzl1NnGun9kOYjK5itrF1Wh17sT!62ipv1vI8omeu0cVRww2Kv!qM*LFgwGlPOnNHj3*VulQOuaoliN4MUUxTA4owDubYZoKAwF*yp7Mg3zq5Ds2!l9Q$$; domain=.live.com;path=/;HTTPOnly= ;version=1,MH=MSFT; domain=.live.com;path=/;version=1,MHW=; expires=Thu, 30-Oct-1980 16:00:00 GMT;domain=.live.com;path=/;version=1,MHList=; expires=Thu, 30-Oct-1980 16:00:00 GMT;domain=.live.com;path=/;version=1,NAP=V=1.9&E=bea&C=zfjCKKBD0TqjZlWGgRTp__NiK08Lme_0XFaiKPaWJ0HDuMi2uCXafQ&W=1;domain=.live.com;path=/,ANON=A=DE389D4D076BF47BCAE4DC05FFFFFFFF&E=c44&W=1;domain=.live.com;path=/,MSPVis=$9;domain=login.live.com;path=/,pres=; expires=Thu, 30-Oct-1980 16:00:00 GMT;domain=.live.com;path=/;version=1,LOpt=0; domain=login.live.com;path=/;version=1,WLSSC=EgBnAQMAAAAEgAAACoAASfCD+8dUptvK4kvFO0gS3mVG28SPT3Jo9Pz2k65r9c9KrN4ISvidiEhxXaPLCSpkfa6fxH3FbdP9UmWAa9KnzKFJu/lQNkZC3rzzMcVUMjbLUpSVVyscJHcfSXmpGGgZK4ZCxPqXaIl9EZ0xWackE4k5zWugX7GR5m/RzakyVIzWAFwA1gD9vwYA7Vazl9QKMk/UCjJPECcAAAoQoAAAFwBjcmlmYW4yMDAzQGhvdG1haWwuY29tAE8AABZjcmlmYW4yMDAzQGhvdG1haWwuY29tAAAACUNOAAYyMTM1OTIAAAZlCAQCAAB3F21AAARDAAR0aWFuAAR3YW5nBMgAAUkAAAAAAAAAAAAAAaOKNpqLi/UAANQKMk/Uf0RPAAAAAAAAAAAAAAAADgA1OC4yNDAuMjM2LjE5AAUAAAAAAAAAAAAAAAABBAABAAABAAABAAAAAAAAAAA=; domain=.live.com;secure= ;path=/;HTTPOnly= ;version=1,MSPSoftVis=@72198325083833620@:@; domain=login.live.com;path=/;version=1
        // here now support parse the un-correct Set-Cookie:
        // MSPRequ=/;Version=1;version&lt=1328770452&id=250915&co=1; path=/;version=1,MSPVis=$9; Version=1;version=1$250915;domain=login.live.com;path=/,MSPSoftVis=@72198325083833620@:@; domain=login.live.com;path=/;version=1,MSPBack=1328770312; domain=login.live.com;path=/;version=1
        public CookieCollection parseSetCookie(string setCookieStr, string curDomain)
        {
            CookieCollection parsedCookies = new CookieCollection();

            // process for expires and Expires field, for it contains ','
            //refer: http://www.yaosansi.com/post/682.html
            // may contains expires or Expires, so following use xpires
            string commaReplaced = Regex.Replace(setCookieStr, @"xpires=\w{3},\s\d{2}-\w{3}-\d{4}", new MatchEvaluator(_processExpireField));
            string[] cookieStrArr = commaReplaced.Split(',');
            foreach (string cookieStr in cookieStrArr)
            {
                Cookie ck = new Cookie();
                // recover it back
                string recoveredCookieStr = Regex.Replace(cookieStr, @"xpires=\w{3}" + replacedChar + @"\s\d{2}-\w{3}-\d{4}", new MatchEvaluator(_recoverExpireField));
                if (parseSingleCookie(recoveredCookieStr, ref ck))
                {
                    if (needAddThisCookie(ck, curDomain))
                    {
                        parsedCookies.Add(ck);
                    }
                }
            }

            return parsedCookies;
        }//parseSetCookie

        // parse Set-Cookie string part into cookies
        // leave current domain to empty, means omit the parsed cookie, which is not set its domain value
        public CookieCollection parseSetCookie(string setCookieStr)
        {
            return parseSetCookie(setCookieStr, "");
        }

        //parse xxx in "new Date(xxx)" of javascript to C# DateTime
        //input example:
        //new Date(1329198041411.84) / new Date(1329440307389.9) / new Date(1329440307483)
        public bool parseJsNewDate(string newDateStr, out DateTime parsedDatetime)
        {
            bool parseOK = false;
            parsedDatetime = new DateTime();

            if ((newDateStr != "") && (newDateStr.Trim() != ""))
            {
                string dateValue = "";
                if (extractSingleStr(@".*new\sDate\((.+?)\).*", newDateStr, out dateValue))
                {
                    double doubleVal = 0.0;
                    if (Double.TryParse(dateValue, out doubleVal))
                    {
                        // try whether is double/int64 milliSecSinceEpoch
                        parsedDatetime = milliSecToDateTime(doubleVal);
                        parseOK = true;
                    }
                    else if (DateTime.TryParse(dateValue, out parsedDatetime))
                    {
                        // try normal DateTime string
                        //refer: http://www.w3schools.com/js/js_obj_date.asp
                        //October 13, 1975 11:13:00
                        //79,5,24 / 79,5,24,11,33,0
                        //1329198041411.3344 / 1329198041411.84 / 1329198041411
                        parseOK = true;
                    }
                }
            }

            return parseOK;
        }

        //parse Javascript string "$Cookie.setCookie(XXX);" to a cookie
        // input example:
        //$Cookie.setCookie('wla42','cHJveHktYmF5LnB2dC1jb250YWN0cy5tc24uY29tfGJ5MioxLDlBOEI4QkY1MDFBMzhBMzYsMSwwLDA=','live.com','/',new Date(1328842189083.44),1);
        //$Cookie.setCookie('wla42','YnkyKjEsOUE4QjhCRjUwMUEzOEEzNiwwLCww','live.com','/',new Date(1329198041411.84),1);
        //$Cookie.setCookie('wla42', 'YnkyKjEsOUE4QjhCRjUwMUEzOEEzNiwwLCww', 'live.com', '/', new Date(1329440307389.9), 1);
        //$Cookie.setCookie('wla42', 'cHJveHktYmF5LnB2dC1jb250YWN0cy5tc24uY29tfGJ5MioxLDlBOEI4QkY1MDFBMzhBMzYsMSwwLDA=', 'live.com', '/', new Date(1329440307483.5), 1);
        //$Cookie.setCookie('wls', 'A|eyJV-t:a*nS', '.live.com', '/', null, 1);
        //$Cookie.setCookie('MSNPPAuth','','.live.com','/',new Date(1327971507311.9),1);
        public bool parseJsSetCookie(string singleSetCookieStr, out Cookie parsedCk)
        {
            bool parseOK = false;
            parsedCk = new Cookie();

            string name = "";
            string value = "";
            string domain = "";
            string path = "";
            string expire = "";
            string secure = "";

            //                                     1=name      2=value     3=domain     4=path   5=expire  6=secure
            string setckP = @"\$Cookie\.setCookie\('(\w+)',\s*'(.*?)',\s*'([\w\.]+)',\s*'(.+?)',\s*(.+?),\s*(\d?)\);";
            Regex setckRx = new Regex(setckP);
            Match foundSetck = setckRx.Match(singleSetCookieStr);
            if (foundSetck.Success)
            {
                name = foundSetck.Groups[1].ToString();
                value = foundSetck.Groups[2].ToString();
                domain = foundSetck.Groups[3].ToString();
                path = foundSetck.Groups[4].ToString();
                expire = foundSetck.Groups[5].ToString();
                secure = foundSetck.Groups[6].ToString();

                // must: name valid and domain is not null
                if (isValidCookieName(name) && (domain != ""))
                {
                    parseOK = true;

                    parsedCk.Name = name;
                    parsedCk.Value = value;
                    parsedCk.Domain = domain;
                    parsedCk.Path = path;

                    // note, here even parse expire field fail
                    //do not consider it must fail to parse the whole cookie
                    if (expire.Trim() == "null")
                    {
                        // do nothing
                    }
                    else
                    {
                        DateTime expireTime;
                        if (parseJsNewDate(expire, out expireTime))
                        {
                            parsedCk.Expires = expireTime;
                        }
                    }

                    if (secure == "1")
                    {
                        parsedCk.Secure = true;
                    }
                    else
                    {
                        parsedCk.Secure = false;
                    }
                }//if (isValidCookieName(name) && (domain != ""))
            }//foundSetck.Success

            return parseOK;
        }

        //check whether a cookie is expired
        //if expired property is set, then just return it value
        //if not set, check whether is a session cookie, if is, then not expired
        //if expires is set, check its real time is expired or not
        public bool isCookieExpired(Cookie ck)
        {
            bool isExpired = false;

            if ((ck != null) && (ck.Name != ""))
            {
                if (ck.Expired)
                {
                    isExpired = true;
                }
                else
                {
                    DateTime initExpiresValue = (new Cookie()).Expires;
                    DateTime expires = ck.Expires;

                    if (expires.Equals(initExpiresValue))
                    {
                        // expires is not set, means this is session cookie, so here no expire
                    }
                    else
                    {
                        // has set expire value
                        if (DateTime.Now.Ticks > expires.Ticks)
                        {
                            isExpired = true;
                        }
                    }
                }
            }
            else
            {
                isExpired = true;
            }

            return isExpired;
        }

        //add a single cookie to cookies, if already exist, update its value
        public void addCookieToCookies(Cookie toAdd, ref CookieCollection cookies, bool overwriteDomain)
        {
            bool found = false;

            if (cookies.Count > 0)
            {
                foreach (Cookie originalCookie in cookies)
                {
                    if (originalCookie.Name == toAdd.Name)
                    {
                        // !!! for different domain, cookie is not same,
                        // so should not set the cookie value here while their domains is not same
                        // only if it explictly need overwrite domain
                        if ((originalCookie.Domain == toAdd.Domain) ||
                            ((originalCookie.Domain != toAdd.Domain) && overwriteDomain))
                        {
                            //here can not force convert CookieCollection to HttpCookieCollection,
                            //then use .remove to remove this cookie then add
                            // so no good way to copy all field value
                            originalCookie.Value = toAdd.Value;

                            originalCookie.Domain = toAdd.Domain;

                            originalCookie.Expires = toAdd.Expires;
                            originalCookie.Version = toAdd.Version;
                            originalCookie.Path = toAdd.Path;

                            //following fields seems should not change
                            //originalCookie.HttpOnly = toAdd.HttpOnly;
                            //originalCookie.Secure = toAdd.Secure;

                            found = true;
                            break;
                        }
                    }
                }
            }

            if (!found)
            {
                if (toAdd.Domain != "")
                {
                    // if add the null domain, will lead to follow req.CookieContainer.Add(cookies) failed !!!
                    cookies.Add(toAdd);
                }
            }

        }//addCookieToCookies

        //add singel cookie to cookies, default no overwrite domain
        public void addCookieToCookies(Cookie toAdd, ref CookieCollection cookies)
        {
            addCookieToCookies(toAdd, ref cookies, false);
        }

        //check whether the cookies contains the ckToCheck cookie
        //support:
        //ckTocheck is Cookie/string
        //cookies is Cookie/string/CookieCollection/string[]
        public bool isContainCookie(object ckToCheck, object cookies)
        {
            bool isContain = false;

            if ((ckToCheck != null) && (cookies != null))
            {
                string ckName = "";
                Type type = ckToCheck.GetType();

                //string typeStr = ckType.ToString();

                //if (ckType.FullName == "System.string")
                if (type.Name.ToLower() == "string")
                {
                    ckName = (string)ckToCheck;
                }
                else if (type.Name == "Cookie")
                {
                    ckName = ((Cookie)ckToCheck).Name;
                }

                if (ckName != "")
                {
                    type = cookies.GetType();

                    // is single Cookie
                    if (type.Name == "Cookie")
                    {
                        if (ckName == ((Cookie)cookies).Name)
                        {
                            isContain = true;
                        }
                    }
                    // is CookieCollection
                    else if (type.Name == "CookieCollection")
                    {
                        foreach (Cookie ck in (CookieCollection)cookies)
                        {
                            if (ckName == ck.Name)
                            {
                                isContain = true;
                                break;
                            }
                        }
                    }
                    // is single cookie name string
                    else if (type.Name.ToLower() == "string")
                    {
                        if (ckName == (string)cookies)
                        {
                            isContain = true;
                        }
                    }
                    // is cookie name string[]
                    else if (type.Name.ToLower() == "string[]")
                    {
                        foreach (string name in ((string[])cookies))
                        {
                            if (ckName == name)
                            {
                                isContain = true;
                                break;
                            }
                        }
                    }
                }
            }

            return isContain;
        }//isContainCookie

        // update cookiesToUpdate to localCookies
        // if omitUpdateCookies designated, then omit cookies of omitUpdateCookies in cookiesToUpdate
        public void updateLocalCookies(CookieCollection cookiesToUpdate, ref CookieCollection localCookies, object omitUpdateCookies)
        {
            if (cookiesToUpdate.Count > 0)
            {
                if (localCookies == null)
                {
                    localCookies = cookiesToUpdate;
                }
                else
                {
                    foreach (Cookie newCookie in cookiesToUpdate)
                    {
                        if (isContainCookie(newCookie, omitUpdateCookies))
                        {
                            // need omit process this
                        }
                        else
                        {
                            addCookieToCookies(newCookie, ref localCookies);
                        }
                    }
                }
            }
        }//updateLocalCookies

        //update cookiesToUpdate to localCookies
        public void updateLocalCookies(CookieCollection cookiesToUpdate, ref CookieCollection localCookies)
        {
            updateLocalCookies(cookiesToUpdate, ref localCookies, null);
        }

        // given a cookie name ckName, get its value from CookieCollection cookies
        public bool getCookieVal(string ckName, ref CookieCollection cookies, out string ckVal)
        {
            //string ckVal = "";
            ckVal = "";
            bool gotValue = false;

            foreach (Cookie ck in cookies)
            {
                if (ck.Name == ckName)
                {
                    gotValue = true;
                    ckVal = ck.Value;
                    break;
                }
            }

            return gotValue;
        }

        /*********************************************************************/
        /* Serialize/Deserialize */
        /*********************************************************************/

        // serialize an object to string
        public bool serializeObjToStr(Object obj, out string serializedStr)
        {
            bool serializeOk = false;
            serializedStr = "";
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, obj);
                serializedStr = System.Convert.ToBase64String(memoryStream.ToArray());

                serializeOk = true;
            }
            catch
            {
                serializeOk = false;
            }

            return serializeOk;
        }

        // deserialize the string to an object
        public bool deserializeStrToObj(string serializedStr, out object deserializedObj)
        {
            bool deserializeOk = false;
            deserializedObj = null;

            try
            {
                byte[] restoredBytes = System.Convert.FromBase64String(serializedStr);
                MemoryStream restoredMemoryStream = new MemoryStream(restoredBytes);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                deserializedObj = binaryFormatter.Deserialize(restoredMemoryStream);

                deserializeOk = true;
            }
            catch
            {
                deserializeOk = false;
            }

            return deserializeOk;
        }

        /*********************************************************************/
        /* HTTP */
        /*********************************************************************/

        /*
         * Note: currently support auto handle cookies
         * currently only support single caller -> multiple caller of these functions will cause cookies accumulated
         * you can clear previous cookies to avoid unexpected result by call clearCurCookies
         */
        public void clearCurCookies()
        {
            if (curCookies != null)
            {
                curCookies = null;
                curCookies = new CookieCollection();
            }
        }

        /* get current cookies */
        public CookieCollection getCurCookies()
        {
            return curCookies;
        }

        /* set current cookies */
        public void setCurCookies(CookieCollection cookies)
        {
            curCookies = cookies;
        }

        /* get url's response
         * */
        public HttpWebResponse getUrlResponse(string url,
                                        Dictionary<string, string> headerDict,
                                        Dictionary<string, string> postDict,
                                        int timeout,
                                        string postDataStr)
        {
            //CookieCollection parsedCookies;

            HttpWebResponse resp = null;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.AllowAutoRedirect = true;
            req.Accept = "*/*";

            //const string gAcceptLanguage = "en-US"; // zh-CN/en-US
            //req.Headers["Accept-Language"] = gAcceptLanguage;

            req.KeepAlive = true;

            //IE8
            const string gUserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E";
            //IE9
            //const string gUserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)"; // x64
            //const string gUserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)"; // x86
            //const string gUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E)";
            //Chrome
            //const string gUserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.99 Safari/533.4";
            //Mozilla Firefox
            //const string gUserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6";
            req.UserAgent = gUserAgent;

            req.Headers["Accept-Encoding"] = "gzip, deflate";
            req.AutomaticDecompression = DecompressionMethods.GZip;

            req.Proxy = null;

            if (timeout > 0)
            {
                req.Timeout = timeout;
            }

            if (curCookies != null)
            {
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.PerDomainCapacity = 40; // following will exceed max default 20 cookie per domain
                req.CookieContainer.Add(curCookies);
            }

            if (headerDict != null)
            {
                foreach (string header in headerDict.Keys)
                {
                    string headerValue = "";
                    if (headerDict.TryGetValue(header, out headerValue))
                    {
                        // following are allow the caller overwrite the default header setting
                        if (header.ToLower() == "referer")
                        {
                            req.Referer = headerValue;
                        }
                        else if (header.ToLower() == "allowautoredirect")
                        {
                            bool isAllow = false;
                            if (bool.TryParse(headerValue, out isAllow))
                            {
                                req.AllowAutoRedirect = isAllow;
                            }
                        }
                        else if (header.ToLower() == "accept")
                        {
                            req.Accept = headerValue;
                        }
                        else if (header.ToLower() == "keepalive")
                        {
                            bool isKeepAlive = false;
                            if (bool.TryParse(headerValue, out isKeepAlive))
                            {
                                req.KeepAlive = isKeepAlive;
                            }
                        }
                        else if (header.ToLower() == "accept-language")
                        {
                            req.Headers["Accept-Language"] = headerValue;
                        }
                        else if (header.ToLower() == "useragent")
                        {
                            req.UserAgent = headerValue;
                        }
                        else
                        {
                            req.Headers[header] = headerValue;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (postDict != null || postDataStr != "")
            {
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                if (postDict != null)
                {
                    postDataStr = quoteParas(postDict);
                }

                //byte[] postBytes = Encoding.GetEncoding("utf-8").GetBytes(postData);
                byte[] postBytes = Encoding.UTF8.GetBytes(postDataStr);
                req.ContentLength = postBytes.Length;

                Stream postDataStream = req.GetRequestStream();
                postDataStream.Write(postBytes, 0, postBytes.Length);
                postDataStream.Close();
            }
            else
            {
                req.Method = "GET";
            }

            //may timeout, has fixed in:
            //http://www.crifan.com/fixed_problem_sometime_httpwebrequest_getresponse_timeout/
            resp = (HttpWebResponse)req.GetResponse();

            updateLocalCookies(resp.Cookies, ref curCookies);

            return resp;
        }

        public HttpWebResponse getUrlResponse(string url,
                                    Dictionary<string, string> headerDict,
                                    Dictionary<string, string> postDict)
        {
            return getUrlResponse(url, headerDict, postDict, 0, "");
        }

        public HttpWebResponse getUrlResponse(string url)
        {
            return getUrlResponse(url, null, null, 0, "");
        }

        // valid charset:"GB18030"/"UTF-8", invliad:"UTF8"
        public string getUrlRespHtml(string url,
                                        Dictionary<string, string> headerDict,
                                        string charset,
                                        Dictionary<string, string> postDict,
                                        int timeout,
                                        string postDataStr)
        {
            string respHtml = "";

            //HttpWebResponse resp = getUrlResponse(url, headerDict, postDict, timeout);
            HttpWebResponse resp = getUrlResponse(url, headerDict, postDict, timeout, postDataStr);

            //long realRespLen = resp.ContentLength;

            StreamReader sr;
            if ((charset != null) && (charset != ""))
            {
                Encoding htmlEncoding = Encoding.GetEncoding(charset);
                sr = new StreamReader(resp.GetResponseStream(), htmlEncoding);
            }
            else
            {
                sr = new StreamReader(resp.GetResponseStream());
            }
            respHtml = sr.ReadToEnd();

            return respHtml;
        }

        public string getUrlRespHtml(string url, Dictionary<string, string> headerDict, string charset, Dictionary<string, string> postDict, string postDataStr)
        {
            return getUrlRespHtml(url, headerDict, charset, postDict, 0, postDataStr);
        }

        public string getUrlRespHtml(string url, Dictionary<string, string> headerDict, string charset, Dictionary<string, string> postDict)
        {
            return getUrlRespHtml(url, headerDict, charset, postDict, 0, "");
        }

        public string getUrlRespHtml(string url, Dictionary<string, string> headerDict, Dictionary<string, string> postDict)
        {
            return getUrlRespHtml(url, headerDict, "", postDict, "");
        }

        public string getUrlRespHtml(string url, Dictionary<string, string> headerDict)
        {
            return getUrlRespHtml(url, headerDict, null);
        }

        public string getUrlRespHtml(string url, string charset, int timeout)
        {
            return getUrlRespHtml(url, null, charset, null, timeout, "");
        }

        public string getUrlRespHtml(string url, string charset)
        {
            return getUrlRespHtml(url, charset, 0);
        }

        public string getUrlRespHtml(string url)
        {
            return getUrlRespHtml(url, "");
        }


        public int getUrlRespStreamBytes(ref Byte[] respBytesBuf,
                                        string url,
                                        Dictionary<string, string> headerDict,
                                        Dictionary<string, string> postDict,
                                        int timeout)
        {
            int curReadoutLen;
            int curBufPos = 0;
            int realReadoutLen = 0;

            try
            {
                //HttpWebResponse resp = getUrlResponse(url, headerDict, postDict, timeout);
                HttpWebResponse resp = getUrlResponse(url, headerDict, postDict);
                int expectReadoutLen = (int)resp.ContentLength;

                Stream binStream = resp.GetResponseStream();
                //int streamDataLen  = (int)binStream.Length; // erro: not support seek operation

                do
                {
                    // here download logic is:
                    // once request, return some data
                    // request multiple time, until no more data
                    curReadoutLen = binStream.Read(respBytesBuf, curBufPos, expectReadoutLen);
                    if (curReadoutLen > 0)
                    {
                        curBufPos += curReadoutLen;
                        expectReadoutLen = expectReadoutLen - curReadoutLen;

                        realReadoutLen += curReadoutLen;
                    }
                } while (curReadoutLen > 0);
            }
            catch
            {
                realReadoutLen = -1;
            }

            return realReadoutLen;
        }

        /*********************************************************************/
        /* File */
        /*********************************************************************/

        //save binary bytes into file
        public bool saveBytesToFile(string fileToSave, ref Byte[] bytes, int dataLen, out string errStr)
        {
            bool saveOk = false;
            errStr = "未知错误！";

            try
            {
                int bufStartPos = 0;
                int bytesToWrite = dataLen;

                FileStream fs;
                fs = File.Create(fileToSave, bytesToWrite);
                fs.Write(bytes, bufStartPos, bytesToWrite);
                fs.Close();

                saveOk = true;
            }
            catch (Exception ex)
            {
                errStr = ex.Message;
            }

            return saveOk;
        }

    }
}
