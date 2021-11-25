using OEECalc.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.JOB
{
    /// <summary>
    /// 设备采集
    /// </summary>
    public class StaticSBCJ_JOB : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                SbcjtjService service = SbcjtjService.Instance;
                service.sbcjtj();
            });
        }
    }
}
