using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    /// <summary>
    /// 设备上传时间间隔配置
    /// </summary>
    public class sys_scsjconf
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 时间间隔（分钟）
        /// </summary>
        public int sjjg { get; set; }
    }
}
