using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LBJOEE;
using LBJOEE.Models;
using LBJOEE.Tools;
namespace LBJOEE.Services
{
    public class LogService: OracleBaseFixture
    {
        private string ip = string.Empty;
        public LogService()
        {
           ip = Tool.GetIpAddress();
        }

        public void Info(string msg,string location="")
        {
            SaveLog(msg, loglevel.info, location);
        }
        public void Error(string msg, string location = "")
        {
            SaveLog(msg, loglevel.error, location);
        }

        public void Warning(string msg, string location = "")
        {
            SaveLog(msg, loglevel.warning, location);
        }

        private void SaveLog(string msg,loglevel loglevel,string location = "")
        {
            try
            {
                sys_log entity = new sys_log();
                entity.rq = DateTime.Now;
                entity.ip = ip;
                entity.loglev = loglevel;
                entity.txt = msg;
                entity.location = location;
                Db.Insert<sys_log>(entity);
            }
            catch (Exception e)
            {
                
            }
        }

        
    }
}
