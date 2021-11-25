using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions.Mapper;
namespace OEECalc.Model
{
    /// <summary>
    /// 设备运行统计
    /// </summary>
    public class sbyxtj
    {
        public int id { get; set; }
        public DateTime sj { get; set; }
        public string sbbh { get; set; }
        public string sbip { get; set; }
        public string sbzt { get; set; }
        public string sbqy { get; set; }
    }

    public class sbyxtj_mapper : ClassMapper<sbyxtj>
    {
        public sbyxtj_mapper()
        {
            Map(t => t.id).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
