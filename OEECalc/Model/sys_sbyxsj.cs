using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    public class sys_sbyxsj
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 计数标记
        /// </summary>
        public int jsflag { get; set; } = 0;
        /// <summary>
        /// 运行开始时间
        /// </summary>
        public DateTime? yxkssj { get; set; }
    }
}
