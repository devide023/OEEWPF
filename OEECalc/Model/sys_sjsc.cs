using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    /// <summary>
    /// 数据上传
    /// </summary>
    public class sys_sjsc
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 计数
        /// </summary>
        public decimal js { get; set; } = 0;
        public string sbzt { get; set; } = string.Empty;
    }
}
