using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OEECalc.Model;
using OEECalc.Services;
using Quartz;
using Quartz.Job;
namespace OEECalc.JOB
{
    /// <summary>
    /// 设备节拍计算
    /// </summary>
    public class CalcJp_JOB : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
           return Task.Run(() =>
            {
                CalcJpService service = CalcJpService.Instance;
                //硬采设备，因为取不到节拍数据，更新采集数据节拍
                var list = ReadConf();
                foreach (var item in list)
                {
                    service.Update_SBJP_Force(item.sbbh,item.hour);
                }
            });
        }

        private IEnumerable<sys_jpcnf> ReadConf()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string fullpath = path + "\\" + "sbjp.json";
                using (StreamReader sr = new StreamReader(fullpath))
                {
                    string json = sr.ReadToEnd();
                    List<sys_jpcnf> list = JsonConvert.DeserializeObject<List<sys_jpcnf>>(json);
                    return list;
                }
            }
            catch (Exception)
            {
                return new List<sys_jpcnf>();
            }
        }
    }
}
