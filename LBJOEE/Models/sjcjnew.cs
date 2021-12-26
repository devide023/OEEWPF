using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
namespace LBJOEE.Models
{
    public class sjcjnew
    {
        /// <summary>
        /// 采集时间
        /// </summary>
        public DateTime cjsj { get; set; } = DateTime.Now;
        /// <summary>
        /// 节拍
        /// </summary>
        public decimal jp { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string sbbh { get; set; }
        /// <summary>
        /// 设备IP
        /// </summary>
        public string sbip { get; set; }
        /// <summary>
        ///加工数
        ///</summary>
        public long jgs { get; set; }
        /// <summary>
        ///循环时间实时
        ///</summary>
        public decimal xhsj { get; set; }
        /// <summary>
        ///运行状态
        ///</summary>
        public string yxzt { get; set; }
        /// <summary>
        ///报警状态
        ///</summary>
        public string bjzt { get; set; }
        /// <summary>
        ///手动状态
        ///</summary>
        public string sdzt { get; set; }
        /// <summary>
        ///自动状态
        ///</summary>
        public string zdzt { get; set; }
        /// <summary>
        ///急停
        ///</summary>
        public string jt { get; set; }
        /// <summary>
        ///开机累计小时
        ///</summary>
        public decimal ljkjsj { get; set; }
        /// <summary>
        ///电子尺当前位置
        ///</summary>
        public decimal dzcdqwz { get; set; }
        /// <summary>
        ///当前压力
        ///</summary>
        public decimal dqyl { get; set; }
        /// <summary>
        ///当前流量
        ///</summary>
        public decimal dqll { get; set; }
        /// <summary>
        ///快速压力
        ///</summary>
        public decimal ksyl { get; set; }
        /// <summary>
        ///增压压力
        ///</summary>
        public decimal zyyl { get; set; }
        /// <summary>
        ///一速速度
        ///</summary>
        public decimal yssd { get; set; }
        /// <summary>
        ///二速速度
        ///</summary>
        public decimal essd { get; set; }
        /// <summary>
        ///三速速度
        ///</summary>
        public decimal sssd { get; set; }
        /// <summary>
        ///三速起点
        ///</summary>
        public decimal sdqd { get; set; }
        /// <summary>
        ///加速时间
        ///</summary>
        public decimal jssj { get; set; }
        /// <summary>
        ///填充行程
        ///</summary>
        public decimal tqxc { get; set; }
        /// <summary>
        ///填充时间
        ///</summary>
        public decimal tcsj { get; set; }
        /// <summary>
        ///料柄厚度
        ///</summary>
        public decimal lbhd { get; set; }
        /// <summary>
        ///料柄位置
        ///</summary>
        public decimal lbwz { get; set; }
        /// <summary>
        ///建压时间
        ///</summary>
        public decimal jysj { get; set; }
        /// <summary>
        ///高速位置
        ///</summary>
        public decimal gswz { get; set; }
        /// <summary>
        ///慢压速度
        ///</summary>
        public decimal mysd { get; set; }
        /// <summary>
        ///高速速度
        ///</summary>
        public decimal gssd { get; set; }
        /// <summary>
        ///铸造压力
        ///</summary>
        public decimal zzyl { get; set; }
        /// <summary>
        ///压射压力
        ///</summary>
        public decimal ysyl { get; set; }
        /// <summary>
        ///压射流量
        ///</summary>
        public decimal ysll { get; set; }
        /// <summary>
        ///质量特性
        ///</summary>
        public string zltx { get; set; }
        /// <summary>
        ///合模力
        ///</summary>
        public decimal hml { get; set; }
        /// <summary>
        ///浇铸剩余厚度
        ///</summary>
        public decimal jzsyhd { get; set; }
        /// <summary>
        ///压缩行程
        ///</summary>
        public decimal ysxc { get; set; }
        /// <summary>
        ///型腔充填时间
        ///</summary>
        public decimal xqctsj { get; set; }
        /// <summary>
        ///总增压时间
        ///</summary>
        public decimal zzysj { get; set; }
        /// <summary>
        ///三速储能压力实际值
        ///</summary>
        public decimal sscnylsjz { get; set; }
        /// <summary>
        ///增压储能压力实际值
        ///</summary>
        public decimal zycnylsjz { get; set; }
        /// <summary>
        ///锁模启动压力
        ///</summary>
        public decimal smqdyl { get; set; }
        /// <summary>
        ///锁模启动流量
        ///</summary>
        public decimal smqdll { get; set; }
        /// <summary>
        ///锁模启动位置
        ///</summary>
        public decimal smqdwz { get; set; }
        /// <summary>
        ///锁模快速压力
        ///</summary>
        public decimal smksyl { get; set; }
        /// <summary>
        ///锁模快速流量
        ///</summary>
        public decimal smksll { get; set; }
        /// <summary>
        ///锁模快速位置
        ///</summary>
        public decimal smkswz { get; set; }
        /// <summary>
        ///锁模低压压力
        ///</summary>
        public decimal skdyyl { get; set; }
        /// <summary>
        ///锁模低压流量
        ///</summary>
        public decimal skdyll { get; set; }
        /// <summary>
        ///锁模低压位置
        ///</summary>
        public decimal smdywz { get; set; }
        /// <summary>
        ///锁模高压压力
        ///</summary>
        public decimal smgyyl { get; set; }
        /// <summary>
        ///锁模高压流量
        ///</summary>
        public decimal smgyll { get; set; }
        /// <summary>
        ///锁模高压位置
        ///</summary>
        public decimal smgywz { get; set; }
        /// <summary>
        ///快速锁模速度
        ///</summary>
        public decimal kssmsd { get; set; }
        /// <summary>
        ///慢速锁模速度
        ///</summary>
        public decimal mssmsd { get; set; }
        /// <summary>
        ///开模缓冲压力
        ///</summary>
        public decimal kmhcyl { get; set; }
        /// <summary>
        ///开模缓冲流量
        ///</summary>
        public decimal kmhcll { get; set; }
        /// <summary>
        ///开模缓冲位置
        ///</summary>
        public decimal kmhcwz { get; set; }
        /// <summary>
        ///开模快速压力
        ///</summary>
        public decimal kmkyyl { get; set; }
        /// <summary>
        ///开模快速流量
        ///</summary>
        public decimal kmksll { get; set; }
        /// <summary>
        ///开模快速位置
        ///</summary>
        public decimal kmkswz { get; set; }
        /// <summary>
        ///开模高压压力
        ///</summary>
        public decimal kmgyyl { get; set; }
        /// <summary>
        ///开模高压流量
        ///</summary>
        public decimal kmgyll { get; set; }
        /// <summary>
        ///开模高压位置
        ///</summary>
        public decimal kmgywz { get; set; }
        /// <summary>
        ///慢速开模速度
        ///</summary>
        public decimal mskmsd { get; set; }
        /// <summary>
        ///快速开模速度
        ///</summary>
        public decimal kskmsd { get; set; }
        /// <summary>
        ///抽芯1进入压力
        ///</summary>
        public decimal cx1jryl { get; set; }
        /// <summary>
        ///抽芯1进入流量
        ///</summary>
        public decimal cx1jrll { get; set; }
        /// <summary>
        ///抽芯1回退压力
        ///</summary>
        public decimal cx1htyl { get; set; }
        /// <summary>
        ///抽芯1回退流量
        ///</summary>
        public decimal cx1htll { get; set; }
        /// <summary>
        ///抽芯2进入压力
        ///</summary>
        public decimal cx2jryl { get; set; }
        /// <summary>
        ///抽芯2进入流量
        ///</summary>
        public decimal cx2jrll { get; set; }
        /// <summary>
        ///抽芯2回退压力
        ///</summary>
        public decimal cx2htyl { get; set; }
        /// <summary>
        ///抽芯2回退流量
        ///</summary>
        public decimal cx2htll { get; set; }
        /// <summary>
        ///抽芯入延时
        ///</summary>
        public decimal cxrys { get; set; }
        /// <summary>
        ///抽芯出延时
        ///</summary>
        public decimal cxcys { get; set; }
        /// <summary>
        ///抽芯储能当前压力
        ///</summary>
        public decimal cxcndqyl { get; set; }
        /// <summary>
        ///抽芯储能压力调零值
        ///</summary>
        public decimal cxcnyltlz { get; set; }
        /// <summary>
        ///抽芯储能压力设定值
        ///</summary>
        public decimal cxcnylsdz { get; set; }
        /// <summary>
        ///快压一速位置
        ///</summary>
        public decimal kyyswz { get; set; }
        /// <summary>
        ///快压二速位置
        ///</summary>
        public decimal kyeswz { get; set; }
        /// <summary>
        ///快压三速位置
        ///</summary>
        public decimal kysswz { get; set; }
        /// <summary>
        ///快压增压位置
        ///</summary>
        public decimal kyzywz { get; set; }
        /// <summary>
        ///快压跟出位置
        ///</summary>
        public decimal kygcwz { get; set; }
        /// <summary>
        ///顶前压力设定
        ///</summary>
        public decimal dqylsd { get; set; }
        /// <summary>
        ///顶前流量设定
        ///</summary>
        public decimal dqllsd { get; set; }
        /// <summary>
        ///顶前延时设定
        ///</summary>
        public decimal dqyssd { get; set; }
        /// <summary>
        ///顶后压力设定
        ///</summary>
        public decimal dhylsd { get; set; }
        /// <summary>
        ///顶后流量设定
        ///</summary>
        public decimal dhllsd { get; set; }
        /// <summary>
        ///顶后延时设定
        ///</summary>
        public decimal dhyssd { get; set; }
        /// <summary>
        ///顶针次数
        ///</summary>
        public long dzcs { get; set; }
        /// <summary>
        ///快速储能压力
        ///</summary>
        public decimal kscnyl { get; set; }
        /// <summary>
        ///增压储能压力
        ///</summary>
        public decimal zycnyl { get; set; }
        /// <summary>
        ///回锤缓冲位置
        ///</summary>
        public decimal hchcwz { get; set; }
        /// <summary>
        ///开模终止位置
        ///</summary>
        public decimal kmzzwz { get; set; }
        /// <summary>
        ///模具位置
        ///</summary>
        public decimal mjwz { get; set; }
        /// <summary>
        ///油温
        ///</summary>
        public decimal yw { get; set; }
        /// <summary>
        ///慢速位置
        ///</summary>
        public decimal mswz { get; set; }
        /// <summary>
        ///模具填充结束
        ///</summary>
        public string mjtcjs { get; set; }
        /// <summary>
        ///压缩行程
        ///</summary>
        public decimal yasxc { get; set; }
        /// <summary>
        ///浇铸活塞
        ///</summary>
        public string jzhs { get; set; }
        /// <summary>
        ///总回路数
        ///</summary>
        public long zhls { get; set; }
        /// <summary>
        ///高速开始
        ///</summary>
        public string gsks { get; set; }
        /// <summary>
        ///高速区间
        ///</summary>
        public string gsqj { get; set; }
        /// <summary>
        ///升压时间
        ///</summary>
        public decimal sysj { get; set; }
        /// <summary>
        ///热模件次数
        ///</summary>
        public long rmjcs { get; set; }
    }

    public class sjcjnew_mapper : ClassMapper<sjcjnew>
    {
        public sjcjnew_mapper()
        {
            Table("sjcj");
            Map(t => t.cjsj).Ignore();
            AutoMap();
        }
    }
}
