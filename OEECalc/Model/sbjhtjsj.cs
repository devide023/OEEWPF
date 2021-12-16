using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    /// <summary>
    /// 设备计划停机时间
    /// </summary>
    public class sbjhtjsj
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
        /// 停机时间（分钟）
        /// </summary>
        public int tjsj { get; set; }
    }
}
