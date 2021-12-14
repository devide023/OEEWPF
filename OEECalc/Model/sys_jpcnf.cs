using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    public class sys_jpcnf
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 当前时间之前的小时数
        /// </summary>
        public int hour { get; set; }
    }
}
