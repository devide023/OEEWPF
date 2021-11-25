using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE.Models
{
    /// <summary>
    /// 记录设备状态变化
    /// </summary>
    public class sbztbhb
    {
        public int id { get; set; }
        public DateTime sj { get; set; }
        public string sbbh { get; set; }
        public string sbzt { get; set; }
        public string sbqy { get; set; }
    }

    public class sbztbhb_maper : ClassMapper<sbztbhb>
    {
        public sbztbhb_maper()
        {
            Map(t => t.id).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
