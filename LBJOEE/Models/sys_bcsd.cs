using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.Models
{
    /// <summary>
    /// 班次时间段
    /// </summary>
    public class sys_bcsd
    {
        /// <summary>
        /// 停机时间是否跨班次
        /// </summary>
        public bool iskbc { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime kssj { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime jssj { get; set; }
    }
}
