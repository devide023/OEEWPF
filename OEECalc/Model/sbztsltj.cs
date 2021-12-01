using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace OEECalc.Model
{
    /// <summary>
    /// 统计设备状态数量
    /// </summary>
    public class sbztsltj
    {
        public long id { get; set; }
        public DateTime sj { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public string sbzt { get; set; }
        /// <summary>
        /// 状态设备数量
        /// </summary>
        public int sl { get; set; }
    }

    public class sbztsltj_mapper : ClassMapper<sbztsltj>
    {
        public sbztsltj_mapper()
        {
            Map(t => t.id).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
