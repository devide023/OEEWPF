using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Configuration;
namespace LBJOEEUpdateService.JOBS
{
    public class CheckOEEProcessJob : IJob
    {
        private ILog log;
        public CheckOEEProcessJob()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                string iepath = ConfigurationManager.AppSettings["iepath"]!=null? ConfigurationManager.AppSettings["iepath"].ToString() : @"C:\Program Files (x86)\Internet Explorer\iexplore.exe";
                string ApplicationURL = ConfigurationManager.AppSettings["appurl"] != null ? ConfigurationManager.AppSettings["appurl"].ToString() : "  http://172.16.201.135:1000/LBJOEE.application";
                Process[] processes = Process.GetProcesses();
                var q = processes.Where(t => t.ProcessName == "LBJOEE");
                if (q.Count() == 0)
                {
                    log.Info("重启应用" + ApplicationURL);
                    Tool.Utils.Openlocalexe(iepath, ApplicationURL);
                    //Tool.Utils.AppStart(ApplicationURL, "");
                    //Process.Start("rundll32.exe", "dfshim.dll,ShOpenVerbApplication " + ApplicationURL);
                }
            });
        }
    }
}
