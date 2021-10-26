using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE
{
    /// <summary>
    /// 数据采集
    /// </summary>
    public class sjcj
    {
        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime? cjsj { get; set; } = DateTime.Now;
        /// <summary>
        /// 节拍
        /// </summary>
        public string jp { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 设备IP
        /// </summary>
        public string sbip { get; set; }
        /// <summary>
        /// 加工数
        /// </summary>
        public string jgs { get; set; }
        /// <summary>
        /// 运行状态
        /// </summary>
        public string yxzt { get; set; }
        /// <summary>
        /// 报警状态
        /// </summary>
        public string bjzt { get; set; }
        /// <summary>
        /// 压射速度
        /// </summary>
        public string yssd { get; set; }
        /// <summary>
        /// 储能压力
        /// </summary>
        public string cnyl { get; set; }
        /// <summary>
        /// 压射曲线
        /// </summary>
        public string ysqx { get; set; }
        /// <summary>
        /// 模次
        /// </summary>
        public string mc { get; set; }
        /// <summary>
        /// 生产时间
        /// </summary>
        public string scsj { get; set; }
        /// <summary>
        /// 慢压射速度
        /// </summary>
        public string myssd { get; set; }
        /// <summary>
        /// 快压射速度
        /// </summary>
        public string kyssd { get; set; }
        /// <summary>
        /// 建压时间
        /// </summary>
        public string jysj { get; set; }
        /// <summary>
        /// 建速时间
        /// </summary>
        public string jssj { get; set; }
        /// <summary>
        /// 模具温度
        /// </summary>
        public string mjwd { get; set; }
        /// <summary>
        /// 料柄厚度
        /// </summary>
        public string lbhd { get; set; }
        /// <summary>
        /// 锁模力
        /// </summary>
        public string sml { get; set; }
        /// <summary>
        /// 系统压力
        /// </summary>
        public string xtyl { get; set; }
        /// <summary>
        /// 增压压力
        /// </summary>
        public string zyyl { get; set; }
        /// <summary>
        /// 氮气瓶压力
        /// </summary>
        public string dqpyl { get; set; }
        /// <summary>
        /// 射料速度
        /// </summary>
        public string slsd { get; set; }
        /// <summary>
        /// 冷却时间
        /// </summary>
        public string lqsj { get; set; }
        /// <summary>
        /// 二块速度
        /// </summary>
        public string eksd { get; set; }
        /// <summary>
        /// 二块位置
        /// </summary>
        public string ekwz { get; set; }
        /// <summary>
        /// 增压流量
        /// </summary>
        public string zyll { get; set; }
        /// <summary>
        /// 浇铸温度
        /// </summary>
        public string jzwd { get; set; }
        /// <summary>
        /// 压射延时
        /// </summary>
        public string ysys { get; set; }
        /// <summary>
        /// 喷雾点位
        /// </summary>
        public string pwdw { get; set; }
        /// <summary>
        /// 顶回压回
        /// </summary>
        public string dhyh { get; set; }
        /// <summary>
        /// 压射距离
        /// </summary>
        public string ysjl { get; set; }
        /// <summary>
        /// 冲头直径
        /// </summary>
        public string ctzj { get; set; }
        /// <summary>
        /// 润滑参数 机铰、锤头
        /// </summary>
        public string rhcs { get; set; }
    }

    public class sjcj_mapper : ClassMapper<sjcj>
    {
        public sjcj_mapper()
        {
            AutoMap();
        }
    }
}
