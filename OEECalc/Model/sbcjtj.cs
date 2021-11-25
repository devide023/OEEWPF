using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace OEECalc.Model
{
    /// <summary>
    /// 设备采集统计
    /// </summary>
    public class sbcjtj
    {
        public int id { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime kssj { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 是否采集
        /// </summary>
        public int sfcj { get; set; }
    }

    public class sbcjtj_mapper : ClassMapper<sbcjtj>
    {
        public sbcjtj_mapper()
        {
            Map(t => t.id).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
