using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
namespace LBJOEE.Tools
{
    public class Tool
    {
        public static string GetIpAddress()
        {
            //获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork" && _IPAddress.ToString().Contains("172.16"))
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }

        public static string GetMacAddress()
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
                return "";
            }
            finally
            {
            }

        }

        public static bool IsPing()
        {
            string url = "172.16.201.135";
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(url, 3000);
            if(pingReply.Status == IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}
