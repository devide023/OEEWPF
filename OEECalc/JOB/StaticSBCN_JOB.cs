using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEECalc.Services;
using Quartz;
using Quartz.Job;
namespace OEECalc.JOB
{
    /// <summary>
    /// 设备产能JOB
    /// </summary>
    public class StaticSBCN_JOB : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                SBCNService service = SBCNService.Instance;
                service.RiCn();
            });
        }
    }
}
