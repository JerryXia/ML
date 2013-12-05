using System;
using System.Management;

namespace ML.WinForm
{
    public class ComputerHardWareInfo
    {
        private string cpuID = string.Empty;
        private string macAddress = string.Empty;
        private string diskID = string.Empty;
        private string ipAddress = string.Empty;
        private string loginUserName = string.Empty;
        public string systemType = string.Empty;
        private string computerName = string.Empty;
        private string totalPhysicalMemory = string.Empty;//单位：M   


        public ComputerHardWareInfo()
        {
            cpuID = GetCpuID();
            macAddress = GetMacAddress();
            diskID = GetDiskID();
            ipAddress = GetIPAddress();
            loginUserName = GetUserName();
            systemType = GetSystemType();
            totalPhysicalMemory = GetTotalPhysicalMemory();
            computerName = GetComputerName();
        }

        public string CpuID
        {
            get
            {
                return this.cpuID;
            }
        }
        public string MacAddress
        {
            get
            {
                return this.macAddress;
            }
        }
        public string DiskID
        {
            get
            {
                return this.diskID;
            }
        }
        public string IpAddress
        {
            get
            {
                return this.ipAddress;
            }
        }
        public string LoginUserName
        {
            get
            {
                return this.loginUserName;
            }
        }
        public string SystemType
        {
            get
            {
                return this.systemType;
            }
        }
        public string TotalPhysicalMemory
        {
            get
            {
                return this.totalPhysicalMemory;
            }
        }
        public string ComputerName
        {
            get
            {
                return this.computerName;
            }
        }


        /// <summary>
        /// 获取cpu序列号
        /// </summary>
        /// <returns></returns>
        string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码   
                string cpuInfo = "";//cpu序列号   
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
                //
            }
        }

        /// <summary>
        /// 获取网卡硬件地址  
        /// </summary>
        /// <returns></returns>
        string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址   
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        string GetIPAddress()
        {
            try
            {
                //获取IP地址   
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        //st=mo["IpAddress"].ToString();   
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        /// <summary>
        /// 获取硬盘ID 
        /// </summary>
        /// <returns></returns>
        string GetDiskID()
        {
            try
            {
                //获取硬盘ID   
                String HDid = "";
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        /// <summary>   
        /// 操作系统的登录用户名   
        /// </summary>   
        /// <returns></returns>   
        string GetUserName()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["UserName"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        /// <summary>   
        /// PC类型   
        /// </summary>   
        /// <returns></returns>   
        string GetSystemType()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["SystemType"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        /// <summary>   
        /// 物理内存   
        /// </summary>   
        /// <returns></returns>   
        string GetTotalPhysicalMemory()
        {
            try
            {

                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["TotalPhysicalMemory"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        /// <summary>   
        ///    电脑名称
        /// </summary>   
        /// <returns></returns>   
        string GetComputerName()
        {
            try
            {
                return System.Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

    }
}
