﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE
{
    /// <summary>
    /// 设备停机
    /// </summary>
    public class sbtj
    {
        public int id { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 停机类型
        /// </summary>
        public string tjlx { get; set; }
        /// <summary>
        /// 停机时间
        /// </summary>
        public int tjsj { get; set; }
        /// <summary>
        /// 停机开始时间
        /// </summary>
        public DateTime tjkssj { get; set; }
        /// <summary>
        /// 停机结束时间
        /// </summary>
        public DateTime tjjssj { get; set; }
        /// <summary>
        /// 停机描述
        /// </summary>
        public string tjms { get; set; }
    }

    public class sbtj_mapper:ClassMapper<sbtj>
    {
        public sbtj_mapper()
        {
            Map(t => t.id).Key(KeyType.TriggerIdentity);
            AutoMap();
        }
    }
}
