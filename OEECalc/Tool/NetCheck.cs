using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Tool
{
    public static class NetCheck
    {
        public static bool IsPing(string url)
        {
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(url, 3000);
            if (pingReply.Status == IPStatus.Success)
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
