using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE
{
    /// <summary>
    /// 设备数据
    /// </summary>
    public class sbsj
    {
        /// <summary>
        /// 接收数据日期
        /// </summary>
        public DateTime rq { get; set; }
        /// <summary>
        /// 接收数据ip地址
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 压射速度
        /// </summary>
        public decimal yssd { get; set; }
        /// <summary>
        /// 储能压力
        /// </summary>
        public decimal cnyl { get; set; }
        /// <summary>
        /// 模次
        /// </summary>
        public int mc { get; set; }
        /// <summary>
        /// 快压射速度
        /// </summary>
        public decimal kyssd { get; set; }
        /// <summary>
        /// 慢压射速度
        /// </summary>
        public decimal myssd { get; set; }
        /// <summary>
        /// 建压时间
        /// </summary>
        public DateTime jysj { get; set; }
        /// <summary>
        /// 建速时间
        /// </summary>
        public decimal jssj { get; set; }
        /// <summary>
        /// 模具温度
        /// </summary>
        public decimal mjwd { get; set; }
        /// <summary>
        /// 料柄厚度
        /// </summary>
        public decimal lbhd { get; set; }
        /// <summary>
        /// 锁模力
        /// </summary>
        public decimal sml { get; set; }
        /// <summary>
        /// 系统压力
        /// </summary>
        public decimal xtyl { get; set; }
        /// <summary>
        /// 增压压力
        /// </summary>
        public decimal zyyl { get; set; }
        /// <summary>
        /// 射料速度
        /// </summary>
        public decimal slsd { get; set; }
        /// <summary>
        /// 一快速度
        /// </summary>
        public decimal yksd { get; set; }
        /// <summary>
        /// 一快位置
        /// </summary>
        public decimal ykwz { get; set; }
        /// <summary>
        /// 二快速度
        /// </summary>
        public decimal eksd { get; set; }
        /// <summary>
        /// 二快位置
        /// </summary>
        public decimal ekwz { get; set; }
        /// <summary>
        /// 压射距离
        /// </summary>
        public decimal ysjl { get; set; }
        /// <summary>
        /// 压射延时
        /// </summary>
        public decimal ysys { get; set; }
        /// <summary>
        /// 冲头直径
        /// </summary>
        public decimal ctzj { get; set; }
        /// <summary>
        /// 浇铸温度
        /// </summary>
        public decimal jzwd { get; set; }
    }

    public class sbsjmaper : ClassMapper<sbsj>
    {
        public sbsjmaper()
        {
            AutoMap();
        }
    }
}
