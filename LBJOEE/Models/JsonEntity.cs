using LBJOEE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE
{
    public class JsonEntity
    {
        public string status { get; set; }
        public string errormsg { get; set; }
        public string errorcode { get; set; }
        public string sbbh { get; set; }
        public string sbip { get; set; }
        /// <summary>
        /// 设备数据采集
        /// </summary>
        public List<itemdata> devicedata { get; set; }
        /// <summary>
        /// 对应数据库字段（采集后的参数）
        /// </summary>
        public sjcjnew SJCJ { get; set; }
        
    }
}
