using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEECalc.Services;
using Quartz;
using Quartz.Job;
using log4net;
using System.Threading;
namespace OEECalc.JOB
{
    /// <summary>
    /// 检查设备是否在线
    /// </summary>
    public class SBZX_JOB : IJob
    {
        private ILog log;
        public SBZX_JOB()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                CheckSBOnLineService service = CheckSBOnLineService.Instance;
                service.CheckOnLine();
                //log.Info("实例值：" + service.GetHashCode() + ",进程id:" + Thread.CurrentThread.ManagedThreadId);
            });
        }
    }
}
