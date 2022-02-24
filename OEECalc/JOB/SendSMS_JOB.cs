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
    /// 发送短信Job
    /// </summary>
    public class SendSMS_JOB : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                SendSMSServic s = new SendSMSServic("sms");
                s.SendMSg();
            });
        }
    }
}
