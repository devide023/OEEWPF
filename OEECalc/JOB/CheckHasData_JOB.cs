using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEECalc.Services;
using Quartz;
using Quartz.Job;
using log4net;
namespace OEECalc.JOB
{
    /// <summary>
    /// 检查开机是否有数据上传
    /// </summary>
    public class CheckHasData_JOB : IJob
    {
        private ILog log;
        public CheckHasData_JOB()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                Check_DataUploadService service = Check_DataUploadService.Instance;
                //log.Info($"HashCode:{service.GetHashCode()}\r");
                service.NewCheck();
            });
        }
    }
}
