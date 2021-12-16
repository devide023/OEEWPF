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
    /// 检查开机是否有数据上传
    /// </summary>
    public class CheckHasData_JOB : IJob
    {
        private Check_DataUploadService service = Check_DataUploadService.Instance;
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                service.Check();
            });
        }
    }
}
