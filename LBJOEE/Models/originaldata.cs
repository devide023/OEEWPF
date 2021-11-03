using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE.Models
{
    public class originaldata
    {
        public int id { get; set; }
        public DateTime? rq { get; set; }
        public string sbbh { get; set; }
        public string ip { get; set; }
        public string json { get; set; }
    }

    public class receivedata_mapper : ClassMapper<originaldata>
    {
        public receivedata_mapper()
        {
            Table("receivedata");
            Map(t => t.id).Key(KeyType.TriggerIdentity);
            AutoMap();
        }
    }
}
