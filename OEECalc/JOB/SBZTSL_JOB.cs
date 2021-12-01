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
    /// 设备运行状态数量统计
    /// </summary>
    public class SBZTSL_JOB : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                SBZTSLTJService service = SBZTSLTJService.Instance;
                service.ZTSL_Static();
            });
        }
    }
}
