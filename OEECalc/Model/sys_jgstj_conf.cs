using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    public class sys_jgstj_conf
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 设备区域
        /// </summary>
        public string sbqy { get; set; }
        /// <summary>
        /// 加工数统计
        /// </summary>
        public int jgstj { get; set; }
    }
}
