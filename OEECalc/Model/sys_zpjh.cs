using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    public class sys_zpjh
    {
        /// <summary>
        /// 设备区域
        /// </summary>
        public string sbqy { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string wlmc { get; set; }
        /// <summary>
        /// 物料编码
        /// </summary>
        public string wlbm { get; set; }
        /// <summary>
        /// 下线数量
        /// </summary>
        public int xxcnt { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public int scsl { get; set; }
        /// <summary>
        /// 压铸模号
        /// </summary>
        public string zmould { get; set; }
        /// <summary>
        /// 压铸模具号
        /// </summary>
        public string zemold { get; set; }
    }
}
