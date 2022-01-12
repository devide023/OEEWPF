using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.Models
{
    public class base_sbxx_conf
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 配置键
        /// </summary>
        public string confkey { get; set; }
        public string confval { get; set; }
    }
}
