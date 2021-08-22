using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE
{
    public class JsonEntity
    {
        public DateTime 日期 { get; set; } = DateTime.Now;
        public string IP { get; set; }
        public string 状态 { get; set; }
        public string 故障信息 { get; set; }
        public string 故障码 { get; set; }
        public sbsj 设备数据 { get; set; }
    }
}
