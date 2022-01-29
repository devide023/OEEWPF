using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.IO;
using LBJOEE.Models;

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
        /// <summary>
        /// 读取文本文件加工数
        /// </summary>
        /// <returns></returns>
        public static long Local2JGS()
        {
            try
            {
                long jgs = 0;
                string path = @"d:\backup\jgs\";
                DirectoryInfo di = new DirectoryInfo(path);
                if (!di.Exists)
                {
                    di.Create();
                }
                string fullpath = path + "jgs.txt";
                using (StreamReader sr = new StreamReader(fullpath))
                {
                   string txt = sr.ReadToEnd();
                   long.TryParse(txt, out jgs);
                }
                return jgs;
            }
            catch (Exception)
            {
                return (long)0;
            }
        }
        /// <summary>
        /// 保存加工数到本地文件
        /// </summary>
        /// <param name="jgs"></param>
        public static void SaveJGS2Local(long jgs)
        {
            try
            {
                string path = @"d:\backup\jgs\";
                DirectoryInfo di = new DirectoryInfo(path);
                if (!di.Exists)
                {
                    di.Create();
                }
                string fullpath = path + "jgs.txt";
                using (StreamWriter sw = new StreamWriter(fullpath))
                {
                    sw.Write(jgs);
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 是否跨班次
        /// </summary>
        /// <param name="tjkssj"></param>
        /// <param name="tjjssj"></param>
        /// <returns></returns>
        public static bool IsCrossBC(DateTime ksrq,DateTime jsrq)
        {
            try
            {
                var bcxx = TimeTool.GetBCInfo(ksrq);
                return !TimeTool.IsSameBC(new sbtj()
                {
                    tjkssj = ksrq,
                    tjjssj = jsrq
                });
            }
            catch (Exception)
            {
                return false;
            }
        }
        
    }
}
