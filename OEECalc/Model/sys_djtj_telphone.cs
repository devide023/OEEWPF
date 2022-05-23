using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Model
{
    public class sys_djtj_telphone
    {
        public string tel { get; set; }
        public string telname { get; set; }
        public int sms_id { get; set; }
        public string sbzt { get; set; }
        public string sbqy { get; set; }
        public DateTime? djkssj { get; set; }
        /// <summary>
        /// 待机时长
        /// </summary>
        public decimal? djsc { get; set; }
        public DateTime? tjkssj { get; set; }
        /// <summary>
        /// 脱机时长
        /// </summary>
        public decimal? tjsc { get; set; }
        /// <summary>
        /// 分类，领导：1，一般员工：0
        /// </summary>
        public int fl { get; set; }
    }
}
