using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEECalc.Services;
using Quartz;
namespace OEECalc.JOB
{
    public class CalcTJSJ_JOB : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                CalcTJSJService service = CalcTJSJService.Instance;
                service.CalcTJSJ();
            });
        }
    }
}
