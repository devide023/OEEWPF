using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEECalc.Services;
using Quartz;
namespace OEECalc.JOB
{
    /// <summary>
    /// 统计设备状态
    /// </summary>
    public class StaticSBZT_JOB : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                SBZTTJService service = SBZTTJService.Instance;
                //设备状态运行时长统计
                service.SBZT_ScTJ();
            });
        }
    }
}
