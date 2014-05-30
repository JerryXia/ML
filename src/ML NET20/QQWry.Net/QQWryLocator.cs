using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace QQWry.Net
{
    public class QQWryLocator
    {
        private static Encoding encoding = Encoding.GetEncoding("GB2312");

        private byte[] data;
        int firstStartIpOffset;
        int lastStartIpOffset;
        int ipCount;

        public int Count { get { return ipCount; } }

        public QQWryLocator(string dataPath)
        {
            using (FileStream fs = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }

            firstStartIpOffset = (int)data[0] + (((int)data[1]) << 8) + (((int)data[2]) << 16) + (((int)data[3]) << 24);
            lastStartIpOffset = (int)data[4] + (((int)data[5]) << 8) + (((int)data[6]) << 16) + (((int)data[7]) << 24);
            ipCount = (lastStartIpOffset - firstStartIpOffset) / 7 + 1;

            if (ipCount <= 1)
            {
                throw new ArgumentException("ip FileDataError");
            }
        }

        public static uint IpToInt(string ip)
        {
            //string[] strArray = ip.Split('.');
            //return (uint.Parse(strArray[0]) << 24) + (uint.Parse(strArray[1]) << 16) + (uint.Parse(strArray[2]) << 8) + uint.Parse(strArray[0]);
            //return (uint)IPAddress.HostToNetworkOrder((int)(IPAddress.Parse(ip).Address));

            byte[] bytes = IPAddress.Parse(ip).GetAddressBytes();
            return (uint)bytes[3] + (((uint)bytes[2]) << 8) + (((uint)bytes[1]) << 16) + (((uint)bytes[0]) << 24);
        }

        public static string IntToIP(uint ip_Int)
        {
            return new IPAddress(ip_Int).ToString();
        }

        public IPLocation Query(string ip)
        {
            IPAddress address = IPAddress.Parse(ip);
            if (address.AddressFamily != AddressFamily.InterNetwork)
            {
                throw new ArgumentException("不支持非IPV4的地址");
            }

            if (IPAddress.IsLoopback(address))
            {
                return new IPLocation() { IP = ip, Country = "本机内部环回地址", Local = string.Empty };
            }

            uint intIP = (uint)IPAddress.HostToNetworkOrder((int)address.Address);

            //if ((((intIP >= IpToInt("0.0.0.0")) && (intIP <= IpToInt("2.255.255.255"))) || ((intIP >= IpToInt("64.0.0.0")) && (intIP <= IpToInt("126.255.255.255")))) ||
            //((intIP >= IpToInt("58.0.0.0")) && (intIP <= IpToInt("60.255.255.255"))))
            //if (intIP <= 50331647 || (intIP >= 1073741824 && intIP <= 2130706431) || (intIP >= 973078528 && intIP <= 1023410175))
            //{
            //    return new IPLocation() { IP = ip, Country = "网络保留地址", Local = string.Empty };
            //}

            IPLocation ipLocation = new IPLocation() { IP = ip };

            uint right = (uint)ipCount;
            uint left = 0;
            uint middle = 0;
            uint startIp = 0;
            uint endIpOff = 0;
            uint endIp = 0;
            int countryFlag = 0;

            while (left < (right - 1))
            {
                middle = (right + left) / 2;
                startIp = GetStartIp(middle, out endIpOff);
                if (intIP == startIp)
                {
                    left = middle;
                    break;
                }
                if (intIP > startIp)
                {
                    left = middle;
                }
                else
                {
                    right = middle;
                }
            }
            startIp = GetStartIp(left, out endIpOff);
            endIp = GetEndIp(endIpOff, out countryFlag);
            if ((startIp <= intIP) && (endIp >= intIP))
            {
                string local;
                ipLocation.Country = GetCountry(endIpOff, countryFlag, out local);
                ipLocation.Local = local;
            }
            else
            {
                ipLocation.Country = "未知";
                ipLocation.Local = string.Empty;
            }
            return ipLocation;
        }

        private uint GetStartIp(uint left, out uint endIpOff)
        {
            int leftOffset = (int)(firstStartIpOffset + (left * 7));
            endIpOff = (uint)data[4 + leftOffset] + (((uint)data[5 + leftOffset]) << 8) + (((uint)data[6 + leftOffset]) << 16);
            return (uint)data[leftOffset] + (((uint)data[1 + leftOffset]) << 8) + (((uint)data[2 + leftOffset]) << 16) + (((uint)data[3 + leftOffset]) << 24);
        }

        private uint GetEndIp(uint endIpOff, out int countryFlag)
        {
            countryFlag = data[4 + endIpOff];
            return (uint)data[endIpOff] + (((uint)data[1 + endIpOff]) << 8) + (((uint)data[2 + endIpOff]) << 16) + (((uint)data[3 + endIpOff]) << 24);
        }

        /// <summary>
        /// Gets the country.
        /// </summary>
        /// <param name="endIpOff">The end ip off.</param>
        /// <param name="countryFlag">The country flag.</param>
        /// <param name="local">The local.</param>
        /// <returns>country</returns>
        private string GetCountry(uint endIpOff, int countryFlag, out string local)
        {
            string country = string.Empty;
            uint offset = endIpOff + 4;
            switch (countryFlag)
            {
                case 1:
                case 2:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    offset = endIpOff + 8;
                    local = (1 == countryFlag) ? string.Empty : GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
                default:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    local = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
            }
            return country;
        }

        private string GetFlagStr(ref uint offset, ref int countryFlag, ref uint endIpOff)
        {
            int flag = 0;
            while (true)
            {
                flag = data[offset];
                //没有重定向
                if (flag != 1 && flag != 2)
                {
                    break;
                }
                if (flag == 2)
                {
                    countryFlag = 2;
                    endIpOff = offset - 4;
                }
                offset = (uint)data[1 + offset] + (((uint)data[2 + offset]) << 8) + (((uint)data[3 + offset]) << 16);
            }
            if (offset < 12)
            {
                return string.Empty;
            }
            return GetStr(ref offset);
        }

        /// <summary>
        /// 读取字符串...
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private string GetStr(ref uint offset)
        {
            byte lowByte = 0;
            byte highByte = 0;
            StringBuilder stringBuilder = new StringBuilder(16);
            while (true)
            {
                lowByte = data[offset++];
                if (lowByte == 0)
                {
                    return stringBuilder.ToString();
                }

                if (lowByte > 0x7f)
                {
                    highByte = data[offset++];
                    if (highByte == 0)
                    {
                        return stringBuilder.ToString();
                    }
                    stringBuilder.Append(encoding.GetString(new byte[] { lowByte, highByte }));
                }
                else
                {
                    stringBuilder.Append((char)lowByte);
                }
            }
        }
    }
}
