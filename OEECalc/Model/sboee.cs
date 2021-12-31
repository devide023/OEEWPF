using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace OEECalc.Model
{
    public class sboee
    {
        /// <summary>
        ///流水号
        ///</summary>
        public int id { get; set; }
        /// <summary>
        ///日期
        ///</summary>
        public DateTime rq { get; set; }
        /// <summary>
        ///设备编号
        ///</summary>
        public string sbbh { get; set; }
        /// <summary>
        ///设备区域
        ///</summary>
        public string sbqy { get; set; }
        /// <summary>
        ///物料编码
        ///</summary>
        public string wlbm { get; set; }
        /// <summary>
        ///物料名称
        ///</summary>
        public string wlmc { get; set; }
        /// <summary>
        ///模具编号
        ///</summary>
        public string mjbh { get; set; }
        /// <summary>
        ///节拍
        ///</summary>
        public decimal jp { get; set; }
        /// <summary>
        ///班次
        ///</summary>
        public string bc { get; set; }
        /// <summary>
        ///合格品数量
        ///</summary>
        public int hgpsl { get; set; }
        /// <summary>
        ///不合格品数量
        ///</summary>
        public int bhgpsl { get; set; }
        /// <summary>
        ///检修时间
        ///</summary>
        public decimal jxsj { get; set; }
        /// <summary>
        ///调试时间
        ///</summary>
        public decimal tssj { get; set; }
        /// <summary>
        ///换模时间
        ///</summary>
        public decimal hmsj { get; set; }
        /// <summary>
        ///修模
        ///</summary>
        public decimal xmsj { get; set; }
        /// <summary>
        ///修机时间
        ///</summary>
        public decimal xjsj { get; set; }
        /// <summary>
        ///待料时间
        ///</summary>
        public decimal dlsj { get; set; }
        /// <summary>
        ///其它时间
        ///</summary>
        public decimal qtsj { get; set; }
        /// <summary>
        ///休息时间
        ///</summary>
        public decimal xxsj { get; set; }
        /// <summary>
        ///工作时间
        ///</summary>
        public decimal gzsj { get; set; }
        /// <summary>
        ///运行时间
        ///</summary>
        public decimal yxsj { get; set; }
        /// <summary>
        ///计划数量
        ///</summary>
        public int jhsl { get; set; }
        /// <summary>
        ///下线数量
        ///</summary>
        public int xxsl { get; set; }
        /// <summary>
        ///压射次数
        ///</summary>
        public long yscs { get; set; }
        /// <summary>
        ///合格率
        ///</summary>
        public decimal hgl { get; set; }
        /// <summary>
        ///利用率
        ///</summary>
        public decimal lyl { get; set; }
        /// <summary>
        ///表现指数
        ///</summary>
        public decimal bxzs { get; set; }
        /// <summary>
        ///OEE
        ///</summary>
        public decimal oee { get; set; }
    }
    public class sboee_mapper : ClassMapper<sboee>
    {
        public sboee_mapper()
        {
            Map(t => t.id).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
