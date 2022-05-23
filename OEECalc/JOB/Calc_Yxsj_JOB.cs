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
    /// 设备运行时间统计
    /// </summary>
    public class Calc_Yxsj_JOB : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                CalcSbyxSjTj service = CalcSbyxSjTj.Instance;
                SBXXService sb = SBXXService.Instance;
                var sblist = sb.Get_SBXX_List().OrderBy(t => t.sbbh);
                foreach (var item in sblist)
                {
                    service.CalcYxSj(DateTime.Today.AddDays(-1), item.sbbh);
                }
            });
        }
    }
}
