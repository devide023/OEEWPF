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
    public class CalcOEE_JOB : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                CalcOEEService service = CalcOEEService.Instance;
                service.SaveOEE();
            });
        }
    }
}
