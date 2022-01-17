using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE.Models
{
    public class sys_eventlog
    {
        public int id { get; set; }
        public string sbbh { get; set; }
        public string sbqy { get; set; }
        public DateTime rq { get; set; }
        public long eventid { get; set; }
        public string message { get; set; }
    }

    public class sys_eventlog_maper : ClassMapper<sys_eventlog>
    {
        public sys_eventlog_maper()
        {
            Map(t => t.id).Key(KeyType.TriggerIdentity);
            AutoMap();
        }
    }
}
