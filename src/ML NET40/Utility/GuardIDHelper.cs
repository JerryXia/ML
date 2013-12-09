using System;

namespace ML.Utility
{
    public class GuardIDHelper
    {
        private const string SALTKEY = "ML";

        private int keepHashCodeLength = 0;
        private string saltKey = String.Empty;

        private int KeepHashCodeLength
        {
            get { return this.keepHashCodeLength; }
            set { this.keepHashCodeLength = value; }
        }
        private string SaltKey
        {
            get { return this.saltKey; }
            set { this.saltKey = value; }
        }

        public GuardIDHelper()
        {
            //ToDo 需要保证HashCode的位数；
            KeepHashCodeLength = 2;
            SaltKey = SALTKEY;
        }

        public GuardIDHelper(int keepHashLength)
        {
            KeepHashCodeLength = keepHashLength;
            SaltKey = SALTKEY;
        }

        public GuardIDHelper(int keepHashLength, string saltKey)
        {
            KeepHashCodeLength = keepHashLength;
            SaltKey = saltKey;
        }

        /// <summary>
        /// 加密成防伪ID
        /// </summary>
        /// <param name="orignalID"></param>
        /// <returns></returns>
        public string Encode(long orignalID)
        {
            //首位是ID的长度的长度，
            string result = string.Format("{0}{1}{2}{3}",
                orignalID.ToString().Length.ToString().Length,
                orignalID.ToString().Length,
                orignalID.ToString(),
                GetSpecCode(orignalID.ToString())
                );
            return result;
        }
        public string Encode(object obj)
        {
            long oriId;
            long.TryParse(obj.ToString(), out oriId);
            return Encode(oriId);
        }
        /// <summary>
        /// 把HashCode的字长补充到标准字长；
        /// </summary>
        /// <param name="orignalID"></param>
        /// <returns></returns>
        private string GetSpecCode(string orignalID)
        {
            string result = (Math.Abs((orignalID + SaltKey).GetHashCode())).ToString();
            if (result.Length >= KeepHashCodeLength)
            {
                result = result.Substring(0, KeepHashCodeLength);
            }
            else
            {
                for (int i = 0; i < KeepHashCodeLength - result.Length; i++)
                {
                    result = "0" + result;
                }
            }
            return result;
        }
        /// <summary>
        /// 解析防伪ID
        /// </summary>
        /// <param name="encodedID"></param>
        /// <returns></returns>
        public long Decode(string encodedID)
        {
            long result;
            int len, lenlen;
            char[] encode = encodedID.ToCharArray();

            if (int.TryParse(encode[0].ToString(), out lenlen) && lenlen <= 2)
            {
                string lenStr = "";
                for (int i = 1; i < lenlen + 1; i++)
                {
                    lenStr += encode[i].ToString();
                }
                if (int.TryParse(lenStr, out len))
                {
                    if (encodedID.Length == 1 + lenlen + len + KeepHashCodeLength)
                    {
                        string orignalID = encodedID.Substring(1 + lenlen, len);
                        string orignalHashCode = encodedID.Substring(lenlen + len + 1);
                        if (orignalHashCode == GetSpecCode(orignalID))
                        {
                            encodedID = orignalID;
                        }
                    }
                }
            }
            else
            {
                return 0;
            }

            //不符合加密条件的直接返回原值，不做处理；
            if (!long.TryParse(encodedID, out result))
            {
                return -1;
            }

            return result;
        }
    }
}
