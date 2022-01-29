using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace OEECalc.Model
{
    public class sbtj
    {
        public int id { get; set; }
        public string sbbh { get; set; }
        public string tjlx { get; set; }
        public int tjsj { get; set; }
        public DateTime tjkssj { get; set; }
        public DateTime tjjssj { get; set; }
        public string tjms { get; set; }
        public string lx { get; set; }
    }

    public class sbtj_mapper : ClassMapper<sbtj>
    {
        public sbtj_mapper()
        {
            Map(t => t.id).Ignore();
            Map(t => t.id).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
