using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace OEECalc.Model
{
    public class sbyxsjtj
    {
        public int id { get; set; }
        public string sbbh { get; set; }
        public DateTime ksrq { get; set; }
        public DateTime jsrq { get; set; }
        public decimal sc { get; set; }
        public DateTime lrsj { get; set; }
    }

    public class sys_yxsjtj_mapper : ClassMapper<sbyxsjtj>
    {
        public sys_yxsjtj_mapper()
        {
            Map(t => t.id).Key(KeyType.TriggerIdentity);
            AutoMap();
        }
    }
}
