using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net;
using OEECalc.Model;
using Newtonsoft.Json;

namespace OEECalc.Tool
{
    public static class ConfigTool
    {
        /// <summary>
        /// 加工数统计，配置文件
        /// </summary>
        public static IEnumerable<sys_jgstj_conf> Read_JGSTJ_Conf()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string fullpath = path + "\\" + "jgsjs.json";
                using (StreamReader sr = new StreamReader(fullpath))
                {
                    string json = sr.ReadToEnd();
                    List<sys_jgstj_conf> list = JsonConvert.DeserializeObject<List<sys_jgstj_conf>>(json);
                    return list;
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger("OEECalc.Tool.ConfigTool").Error(e.Message);
                return new List<sys_jgstj_conf>();
            }
        }
    }
}
