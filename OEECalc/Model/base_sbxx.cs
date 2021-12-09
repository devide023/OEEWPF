using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace OEECalc.Model
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class base_sbxx
    {
        /// <summary>
        ///设备编号
        ///</summary>
        public string sbbh { get; set; }
        /// <summary>
        ///设备名称
        ///</summary>
        public string sbmc { get; set; }
        /// <summary>
        ///设备型号
        ///</summary>
        public string sbxh { get; set; }
        /// <summary>
        ///设备品牌
        ///</summary>
        public string sbpp { get; set; }
        /// <summary>
        ///设备状态
        ///</summary>
        public string sbzt { get; set; }
        /// <summary>
        ///是否检修
        ///</summary>
        public string sfjx { get; set; }
        /// <summary>
        ///是否换模
        ///</summary>
        public string sfhm { get; set; }
        /// <summary>
        ///是否故障
        ///</summary>
        public string sfgz { get; set; }
        /// <summary>
        ///是否缺料
        ///</summary>
        public string sfql { get; set; }
        /// <summary>
        ///是否其他停机
        ///</summary>
        public string sfqttj { get; set; }
        /// <summary>
        ///检修开始时间
        ///</summary>
        public DateTime? jxkssj { get; set; }
        /// <summary>
        ///换模开始时间
        ///</summary>
        public DateTime? hmkssj { get; set; }
        /// <summary>
        ///故障开始时间
        ///</summary>
        public DateTime? gzkssj { get; set; }
        /// <summary>
        ///缺料开始时间
        ///</summary>
        public DateTime? qlkssj { get; set; }
        /// <summary>
        ///其他停机开始时间
        ///</summary>
        public DateTime? qttjkssj { get; set; }
        /// <summary>
        ///设备区域
        ///</summary>
        public string sbqy { get; set; }
        /// <summary>
        ///一体机ip地址
        ///</summary>
        public string ip { get; set; }
        /// <summary>
        ///录入人
        ///</summary>
        public string lrr { get; set; }
        /// <summary>
        ///录入时间
        ///</summary>
        public DateTime lrsj { get; set; }
        /// <summary>
        ///停机描述
        ///</summary>
        public string tjms { get; set; }
        /// <summary>
        ///socket端口号
        ///</summary>
        public int port { get; set; }
        /// <summary>
        ///是否采集故障
        ///</summary>
        public string cjgz { get; set; }
        /// <summary>
        ///是否启用日志
        ///</summary>
        public string log { get; set; }
        /// <summary>
        ///是否更新
        ///</summary>
        public string isupdate { get; set; }
        /// <summary>
        ///是否保存原始数据
        ///</summary>
        public int issaveyssj { get; set; }
        /// <summary>
        ///状态更新时间
        ///</summary>
        public DateTime gxsj { get; set; }
        /// <summary>
        ///删除标志
        ///</summary>
        public string scbz { get; set; }
        /// <summary>
        /// 待机开始时间
        /// </summary>
        public DateTime? djkssj { get; set; }
        /// <summary>
        /// 停机开始时间
        /// </summary>
        public DateTime? tjkssj { get; set; }

        public string sfxm { get; set; }
        public string sfts { get; set; }
        public DateTime? xmkssj { get; set; }
        public DateTime? tskssj { get; set; }

    }

    public class base_sbxx_mapper : ClassMapper<base_sbxx>
    {
        public base_sbxx_mapper()
        {
            Map(t => t.sbbh).Key(KeyType.Assigned);
            AutoMap();
        }
    }
}
