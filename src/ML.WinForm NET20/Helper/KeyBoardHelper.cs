using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace ML.WinForm
{
    public class KeyBoardHelper
    {
        #region dll调用

        [DllImport("User32.dll")]
        static private extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtroInfo);

        [DllImport("User32.dll")]
        static private extern short GetKeyState(int nVk);

        #endregion


        #region 键盘的方法

        const uint EXTENDEDKEY = 0x1;
        const uint KEYUP = 0x2;
        const uint KEYDOWN = 0x00;
        private static Dictionary<string, byte> _keyDic = new Dictionary<string, byte>();

        public KeyBoardHelper()
        {
            if (_keyDic.Count == 0)
            {
                _keyDic.Add("{Backspace}", 0x08);
                _keyDic.Add("{Tab}", 0x09);
                _keyDic.Add("{Clear}", 0x0C);//
                _keyDic.Add("{Enter}", 0x0D);
                _keyDic.Add("{Shift_L}", 0x10);
                _keyDic.Add("{Ctrl_L}", 0x11);
                _keyDic.Add("{Alt_L}", 0x12);
                _keyDic.Add("{Pause}", 0x13);
                _keyDic.Add("{CapsLock}", 0x14);
                _keyDic.Add("{Esc}", 0x1B);
                _keyDic.Add("{Space}", 0x20);
                _keyDic.Add("{PgUp}", 0x21);
                _keyDic.Add("{PgDn}", 0x22);
                _keyDic.Add("{End}", 0x23);
                _keyDic.Add("{Home}", 0x24);
                _keyDic.Add("{Left}", 0x25);
                _keyDic.Add("{Up}", 0x26);
                _keyDic.Add("{Right}", 0x27);
                _keyDic.Add("{Down}", 0x28);
                _keyDic.Add("{Select}", 0x29);//
                _keyDic.Add("{Print}", 0x2A);//
                _keyDic.Add("{Execute}", 0x2B);//
                _keyDic.Add("{ScreenShot}", 0x2C);
                _keyDic.Add("{Insert}", 0x2D);
                _keyDic.Add("{Delete}", 0x2E);//
                _keyDic.Add("{Help}", 0x2F);//

                _keyDic.Add("{A}", 0x41);
                _keyDic.Add("{B}", 0x42);
                _keyDic.Add("{C}", 0x43);
                _keyDic.Add("{D}", 0x44);
                _keyDic.Add("{E}", 0x45);
                _keyDic.Add("{F}", 0x46);
                _keyDic.Add("{G}", 0x47);
                _keyDic.Add("{H}", 0x48);
                _keyDic.Add("{I}", 0x49);
                _keyDic.Add("{J}", 0x4A);
                _keyDic.Add("{K}", 0x4B);
                _keyDic.Add("{L}", 0x4C);
                _keyDic.Add("{M}", 0x4D);
                _keyDic.Add("{N}", 0x4E);
                _keyDic.Add("{O}", 0x4F);
                _keyDic.Add("{P}", 0x50);
                _keyDic.Add("{Q}", 0x51);
                _keyDic.Add("{R}", 0x52);
                _keyDic.Add("{S}", 0x53);
                _keyDic.Add("{T}", 0x54);
                _keyDic.Add("{U}", 0x55);
                _keyDic.Add("{V}", 0x56);
                _keyDic.Add("{W}", 0x57);
                _keyDic.Add("{X}", 0x58);
                _keyDic.Add("{Y}", 0x59);
                _keyDic.Add("{Z}", 0x5A);
                _keyDic.Add("{Win_L}", 0x5B);
                _keyDic.Add("{Win_R}", 0x5C);

                _keyDic.Add("{0}", 0x60);
                _keyDic.Add("{1}", 0x61);
                _keyDic.Add("{2}", 0x62);
                _keyDic.Add("{3}", 0x63);
                _keyDic.Add("{4}", 0x64);
                _keyDic.Add("{5}", 0x65);
                _keyDic.Add("{6}", 0x66);
                _keyDic.Add("{7}", 0x67);
                _keyDic.Add("{8}", 0x68);
                _keyDic.Add("{9}", 0x69);
                _keyDic.Add("{*}", 0x6A);
                _keyDic.Add("{+}", 0x6B);
                _keyDic.Add("{-}", 0x6D);
                _keyDic.Add("{/}", 0x6F);

                _keyDic.Add("{F1}", 0x70);
                _keyDic.Add("{F2}", 0x71);
                _keyDic.Add("{F3}", 0x72);
                _keyDic.Add("{F4}", 0x73);
                _keyDic.Add("{F5}", 0x74);
                _keyDic.Add("{F6}", 0x75);
                _keyDic.Add("{F7}", 0x76);
                _keyDic.Add("{F8}", 0x77);
                _keyDic.Add("{F9}", 0x78);
                _keyDic.Add("{F10}", 0x79);
                _keyDic.Add("{F11}", 0x7A);
                _keyDic.Add("{F12}", 0x7B);

                _keyDic.Add("{NumLock}", 0x90);
                _keyDic.Add("{ScrollLock}", 0x91);

                _keyDic.Add("{Menu}", 0xA5);
            }
        }

        /// <summary>
        /// 键按下
        /// </summary>
        /// <param name="keyVal"></param>
        public void KeyDown(string keyVal)
        {
            byte vk;
            if (MapKeyToASCII(keyVal, out vk) || MapKey(keyVal, out vk))
            {
                KeyDown(vk);
            }
        }
        public void KeyDown(byte vk)
        {
            if (GetKeyState((int)vk) != 1)
            {
                keybd_event(vk, 0, EXTENDEDKEY | KEYDOWN, 0);
                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 组合键按下
        /// </summary>
        /// <param name="keyVals"></param>
        public void ComposeKeyDown(string[] keyVals)
        {
            foreach (string keyVal in keyVals)
            {
                byte vk;
                if (MapKey(keyVal, out vk) || MapKeyToASCII(keyVal, out vk))
                {
                    KeyDown(vk);
                }
            }
            foreach (string keyVal in keyVals)
            {
                byte vk;
                if (MapKey(keyVal, out vk) || MapKeyToASCII(keyVal, out vk))
                {
                    KeyUp(vk);
                }
            }
        }

        /// <summary>
        /// 键升起
        /// </summary>
        /// <param name="keyVal"></param>
        public void KeyUp(string keyVal)
        {
            byte vk;
            if (MapKeyToASCII(keyVal, out vk) || MapKey(keyVal, out vk))
            {
                KeyUp(vk);
            }
        }
        public void KeyUp(byte vk)
        {
            if (GetKeyState((int)vk) == 1)
            {
                keybd_event(vk, 0, EXTENDEDKEY | KEYUP, 0);
                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 键盘点击
        /// </summary>
        /// <param name="keyStr"></param>
        public void KeyPress(string keyStr)
        {
            byte vk;
            if (MapKeyToASCII(keyStr, out vk) || MapKey(keyStr, out vk))
                KeyPress(vk);
        }
        public void KeyPress(byte vk)
        {
            KeyDown(vk);
            KeyUp(vk);
        }

        /// <summary>
        /// this function is a utility function for send mutli char
        /// </summary>
        /// <param name="str"></param>
        public void SendKeys(string str)
        {
            ArrayList vkArr = SplitKeyValue(str);
            if (vkArr.Count > 0)
            {
                foreach (object vk in vkArr)
                    KeyPress((byte)vk);
            }
        }

        #endregion

        /// <summary>
        /// map the string to virtual key value
        /// </summary>
        /// <param name="keyVal">string key value</param>
        /// <param name="bVk">virtual key value</param>
        /// <returns></returns>
        private bool MapKey(string keyVal, out byte bVk)
        {
            return _keyDic.TryGetValue(keyVal, out bVk);
        }

        private bool MapKeyToASCII(string keyVal, out byte bVk)
        {
            if (keyVal.Length == 1)
            {
                char[] ch = keyVal.ToCharArray();
                bVk = (byte)ch[0];
                return true;
            }
            else
            {
                bVk = 0xFF;
                return false;
            }
        }

        /// <summary>
        /// split the string to virtual key array
        /// the string maybe include the system
        /// </summary>
        /// <param name="keyStr"></param>
        /// <returns>virtual key array , byte type</returns>
        private ArrayList SplitKeyValue(string keyStr)
        {
            ArrayList keyArray = new ArrayList();
            bool sysKeyFlag = false;
            StringBuilder sysKey = new StringBuilder();

            foreach (char ch in keyStr.ToCharArray())
            {
                if (ch == '{' && !sysKeyFlag)
                {
                    sysKeyFlag = true;
                    sysKey.Append(ch);
                }
                else if (ch == '{' && sysKeyFlag)  // case  "{aa{", we need add "{aa" to array
                {
                    string sysKeyStr = sysKey.ToString();

                    foreach (char innerCh in sysKeyStr.ToCharArray())
                        keyArray.Add((byte)innerCh);

                    sysKey.Remove(0, sysKey.Length);
                    sysKey.Append(ch);
                }
                else if (ch == '}' && sysKeyFlag) // case "{Ctrl}"
                {
                    sysKeyFlag = false;
                    sysKey.Append(ch);

                    // there is a system key as "{Ctrl}"
                    string sysKeyStr = sysKey.ToString();
                    byte vk;
                    if (MapKey(sysKeyStr, out vk))
                    {
                        keyArray.Add(vk);
                    }
                    else
                    {
                        // case "{abcd}"
                        foreach (char innerCh in sysKeyStr.ToCharArray())
                            keyArray.Add((byte)innerCh);
                    }

                    sysKey.Remove(0, sysKey.Length);
                }
                else
                {
                    if (sysKeyFlag)
                        sysKey.Append(ch);
                    else
                        keyArray.Add((byte)ch);
                }
            }

            return keyArray;
        }
    }
}
