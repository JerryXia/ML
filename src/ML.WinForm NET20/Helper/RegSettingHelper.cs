using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace ML.WinForm
{
    /// <summary>
    /// 注册表操作类
    /// </summary>
    public class RegSettingHelper
    {
        /// <summary>
        /// 读取路径为keypath，键名为keyname的注册表键值，缺省返回def
        /// </summary>
        /// <param name="rootkey"></param>
        /// <param name="keypath">路径</param>
        /// <param name="keyname">键名</param>
        /// <param name="rtn">默认为null</param>
        /// <returns></returns>        
        public static bool GetRegVal(RegistryKey rootkey, string keypath, string keyname, out string rtn)
        {
            rtn = string.Empty;
            try
            {
                RegistryKey key = rootkey.OpenSubKey(keypath);
                rtn = key.GetValue(keyname).ToString();
                key.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 设置路径为keypath，键名为keyname的注册表键值为keyval
        /// </summary>
        /// <param name="rootkey"></param>
        /// <param name="keypath"></param>
        /// <param name="keyname"></param>
        /// <param name="keyval"></param>
        /// <returns></returns>
        public static bool SetRegVal(RegistryKey rootkey, string keypath, string keyname, string keyval)
        {
            try
            {
                RegistryKey key = rootkey.OpenSubKey(keypath, true);
                if (key == null)
                    key = rootkey.CreateSubKey(keypath);
                key.SetValue(keyname, (object)keyval);
                key.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// 创建路径为keypath的键
        private RegistryKey CreateRegKey(RegistryKey rootkey, string keypath)
        {
            try
            {
                return rootkey.CreateSubKey(keypath);
            }
            catch
            {
                return null;
            }
        }
        /// 删除路径为keypath的子项
        private bool DelRegSubKey(RegistryKey rootkey, string keypath)
        {
            try
            {
                rootkey.DeleteSubKey(keypath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// 删除路径为keypath的子项及其附属子项
        private bool DelRegSubKeyTree(RegistryKey rootkey, string keypath)
        {
            try
            {
                rootkey.DeleteSubKeyTree(keypath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// 删除路径为keypath下键名为keyname的键值
        private bool DelRegKeyVal(RegistryKey rootkey, string keypath, string keyname)
        {
            try
            {
                RegistryKey key = rootkey.OpenSubKey(keypath, true);
                key.DeleteValue(keyname);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /***************************
         * IE浏览器注册表操作
         ***************************/

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="NodesName"></param>
        /// <returns></returns>
        public static bool AddNodes(string NodesPath, string VName, string Vvalue)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine;
                RegistryKey key2 = key.CreateSubKey(NodesPath);
                key2.SetValue(VName, Vvalue);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 删除键值
        /// </summary>
        /// <param name="NodesPath"></param>
        /// <param name="KeyNames"></param>
        /// <returns></returns>
        public static bool RegeditKeyDel(string NodesPath, List<string> KeyNames)
        {
            RegistryKey hkml = Registry.LocalMachine;
            try
            {
                string[] subkeyNames;
                RegistryKey software = hkml.OpenSubKey(NodesPath, true);
                subkeyNames = software.GetValueNames();
                //取得该项下所有键值的名称的序列，并传递给预定的数组中
                foreach (string keyName in subkeyNames)
                {
                    if (KeyNames.Contains(keyName))  //判断键值的名称
                    {
                        software.DeleteValue(keyName);
                    }
                }
                if (KeyNames.Contains("默认"))
                {
                    software.SetValue("", "");
                }
                hkml.Close();
                return true;
            }
            catch { hkml.Close(); return false; }
        }
        /// <summary>
        /// 删除所有键值
        /// </summary>
        /// <param name="NodesPath"></param>
        /// <param name="KeyNames"></param>
        /// <returns></returns>
        public static bool RegeditKeyDel(string NodesPath)
        {
            RegistryKey hkml = Registry.LocalMachine;
            try
            {
                string[] subkeyNames;
                RegistryKey software = hkml.OpenSubKey(NodesPath, true);
                subkeyNames = software.GetValueNames();
                //取得该项下所有键值的名称的序列，并传递给预定的数组中
                foreach (string keyName in subkeyNames)
                {
                    if (keyName.Trim() == "" || keyName.Trim() == "@")
                        software.SetValue("", "");
                    else
                        software.DeleteValue(keyName);
                }

                hkml.Close();
                return true;
            }
            catch { hkml.Close(); return false; }
        }

        /// <summary>
        /// 设置注册表键值
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool SetReg(List<string> keys)
        {
            try
            {
                RegeditKeyDel("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\5.0\\User Agent\\Post Platform");
                if (keys.Count > 1 && keys[0].Contains("Mozilla/"))
                {
                    AddNodes("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\5.0\\User Agent", "", keys[0].Trim());
                    AddNodes("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\5.0\\User Agent", "", keys[0].Trim());
                }
                else
                    return false;
                int tempint = -1;
                tempint = keys.FindIndex(delegate(string C) { return C.Contains("MSIE"); });
                if (tempint >= 0)
                {
                    AddNodes("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\5.0\\User Agent", "Version", keys[tempint].Trim());
                    AddNodes("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\5.0\\User Agent", "Version", keys[tempint].Trim());
                }
                tempint = -1;
                tempint = keys.FindIndex(delegate(string C) { return C.Contains("Windows NT"); });
                if (tempint >= 0)
                {
                    AddNodes("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\5.0\\User Agent", "Platform", keys[tempint].Trim());
                }

                foreach (string i in keys.ToArray())
                {
                    if (i.Contains("Mozilla/"))
                        continue;
                    if (i.Contains("MSIE"))
                        continue;
                    if (i.Contains("Windows NT"))
                        continue;
                    if (i.Contains("compatible"))
                        continue;
                    if (i.Contains("Trident"))
                        continue;
                    AddNodes("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\5.0\\User Agent\\Post Platform", i.Trim(), "");
                }
                return true;
            }
            catch { return false; }
        }
    }
}
