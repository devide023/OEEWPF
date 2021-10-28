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
        ///加工数
        ///</summary>
        public string jgs { get; set; }
        /// <summary>
        ///循环时间实时
        ///</summary>
        public string xhsj { get; set; }
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
        public string ljkjsj { get; set; }
        /// <summary>
        ///电子尺当前位置
        ///</summary>
        public string dzcdqwz { get; set; }
        /// <summary>
        ///当前压力
        ///</summary>
        public string dqyl { get; set; }
        /// <summary>
        ///当前流量
        ///</summary>
        public string dqll { get; set; }
        /// <summary>
        ///快速压力
        ///</summary>
        public string ksyl { get; set; }
        /// <summary>
        ///增压压力
        ///</summary>
        public string zyyl { get; set; }
        /// <summary>
        ///一速速度
        ///</summary>
        public string yssd { get; set; }
        /// <summary>
        ///二速速度
        ///</summary>
        public string essd { get; set; }
        /// <summary>
        ///三速速度
        ///</summary>
        public string sssd { get; set; }
        /// <summary>
        ///三速起点
        ///</summary>
        public string sdqd { get; set; }
        /// <summary>
        ///加速时间
        ///</summary>
        public string jssj { get; set; }
        /// <summary>
        ///填充行程
        ///</summary>
        public string tqxc { get; set; }
        /// <summary>
        ///填充时间
        ///</summary>
        public string tcsj { get; set; }
        /// <summary>
        ///料柄厚度
        ///</summary>
        public string lbhd { get; set; }
        /// <summary>
        ///料柄位置
        ///</summary>
        public string lbwz { get; set; }
        /// <summary>
        ///建压时间
        ///</summary>
        public string jysj { get; set; }
        /// <summary>
        ///高速位置
        ///</summary>
        public string gswz { get; set; }
        /// <summary>
        ///慢压速度
        ///</summary>
        public string mysd { get; set; }
        /// <summary>
        ///高速速度
        ///</summary>
        public string gssd { get; set; }
        /// <summary>
        ///铸造压力
        ///</summary>
        public string zzyl { get; set; }
        /// <summary>
        ///压射压力
        ///</summary>
        public string ysyl { get; set; }
        /// <summary>
        ///压射流量
        ///</summary>
        public string ysll { get; set; }
        /// <summary>
        ///质量特性
        ///</summary>
        public string zltx { get; set; }
        /// <summary>
        ///合模力
        ///</summary>
        public string hml { get; set; }
        /// <summary>
        ///浇铸剩余厚度
        ///</summary>
        public string jzsyhd { get; set; }
        /// <summary>
        ///压缩行程
        ///</summary>
        public string ysxc { get; set; }
        /// <summary>
        ///型腔充填时间
        ///</summary>
        public string xqctsj { get; set; }
        /// <summary>
        ///总增压时间
        ///</summary>
        public string zzysj { get; set; }
        /// <summary>
        ///三速储能压力实际值
        ///</summary>
        public string sscnylsjz { get; set; }
        /// <summary>
        ///增压储能压力实际值
        ///</summary>
        public string zycnylsjz { get; set; }
        /// <summary>
        ///锁模启动压力
        ///</summary>
        public string smqdyl { get; set; }
        /// <summary>
        ///锁模启动流量
        ///</summary>
        public string smqdll { get; set; }
        /// <summary>
        ///锁模启动位置
        ///</summary>
        public string smqdwz { get; set; }
        /// <summary>
        ///锁模快速压力
        ///</summary>
        public string smksyl { get; set; }
        /// <summary>
        ///锁模快速流量
        ///</summary>
        public string smksll { get; set; }
        /// <summary>
        ///锁模快速位置
        ///</summary>
        public string smkswz { get; set; }
        /// <summary>
        ///锁模低压压力
        ///</summary>
        public string skdyyl { get; set; }
        /// <summary>
        ///锁模低压流量
        ///</summary>
        public string skdyll { get; set; }
        /// <summary>
        ///锁模低压位置
        ///</summary>
        public string smdywz { get; set; }
        /// <summary>
        ///锁模高压压力
        ///</summary>
        public string smgyyl { get; set; }
        /// <summary>
        ///锁模高压流量
        ///</summary>
        public string smgyll { get; set; }
        /// <summary>
        ///锁模高压位置
        ///</summary>
        public string smgywz { get; set; }
        /// <summary>
        ///快速锁模速度
        ///</summary>
        public string kssmsd { get; set; }
        /// <summary>
        ///慢速锁模速度
        ///</summary>
        public string mssmsd { get; set; }
        /// <summary>
        ///开模缓冲压力
        ///</summary>
        public string kmhcyl { get; set; }
        /// <summary>
        ///开模缓冲流量
        ///</summary>
        public string kmhcll { get; set; }
        /// <summary>
        ///开模缓冲位置
        ///</summary>
        public string kmhcwz { get; set; }
        /// <summary>
        ///开模快速压力
        ///</summary>
        public string kmkyyl { get; set; }
        /// <summary>
        ///开模快速流量
        ///</summary>
        public string kmksll { get; set; }
        /// <summary>
        ///开模快速位置
        ///</summary>
        public string kmkswz { get; set; }
        /// <summary>
        ///开模高压压力
        ///</summary>
        public string kmgyyl { get; set; }
        /// <summary>
        ///开模高压流量
        ///</summary>
        public string kmgyll { get; set; }
        /// <summary>
        ///开模高压位置
        ///</summary>
        public string kmgywz { get; set; }
        /// <summary>
        ///慢速开模速度
        ///</summary>
        public string mskmsd { get; set; }
        /// <summary>
        ///快速开模速度
        ///</summary>
        public string kskmsd { get; set; }
        /// <summary>
        ///抽芯1进入压力
        ///</summary>
        public string cx1jryl { get; set; }
        /// <summary>
        ///抽芯1进入流量
        ///</summary>
        public string cx1jrll { get; set; }
        /// <summary>
        ///抽芯1回退压力
        ///</summary>
        public string cx1htyl { get; set; }
        /// <summary>
        ///抽芯1回退流量
        ///</summary>
        public string cx1htll { get; set; }
        /// <summary>
        ///抽芯2进入压力
        ///</summary>
        public string cx2jryl { get; set; }
        /// <summary>
        ///抽芯2进入流量
        ///</summary>
        public string cx2jrll { get; set; }
        /// <summary>
        ///抽芯2回退压力
        ///</summary>
        public string cx2htyl { get; set; }
        /// <summary>
        ///抽芯2回退流量
        ///</summary>
        public string cx2htll { get; set; }
        /// <summary>
        ///抽芯入延时
        ///</summary>
        public string cxrys { get; set; }
        /// <summary>
        ///抽芯出延时
        ///</summary>
        public string cxcys { get; set; }
        /// <summary>
        ///抽芯储能当前压力
        ///</summary>
        public string cxcndqyl { get; set; }
        /// <summary>
        ///抽芯储能压力调零值
        ///</summary>
        public string cxcnyltlz { get; set; }
        /// <summary>
        ///抽芯储能压力设定值
        ///</summary>
        public string cxcnylsdz { get; set; }
        /// <summary>
        ///快压一速位置
        ///</summary>
        public string kyyswz { get; set; }
        /// <summary>
        ///快压二速位置
        ///</summary>
        public string kyeswz { get; set; }
        /// <summary>
        ///快压三速位置
        ///</summary>
        public string kysswz { get; set; }
        /// <summary>
        ///快压增压位置
        ///</summary>
        public string kyzywz { get; set; }
        /// <summary>
        ///快压跟出位置
        ///</summary>
        public string kygcwz { get; set; }
        /// <summary>
        ///顶前压力设定
        ///</summary>
        public string dqylsd { get; set; }
        /// <summary>
        ///顶前流量设定
        ///</summary>
        public string dqllsd { get; set; }
        /// <summary>
        ///顶前延时设定
        ///</summary>
        public string dqyssd { get; set; }
        /// <summary>
        ///顶后压力设定
        ///</summary>
        public string dhylsd { get; set; }
        /// <summary>
        ///顶后流量设定
        ///</summary>
        public string dhllsd { get; set; }
        /// <summary>
        ///顶后延时设定
        ///</summary>
        public string dhyssd { get; set; }
        /// <summary>
        ///顶针次数
        ///</summary>
        public string dzcs { get; set; }
        /// <summary>
        ///快速储能压力
        ///</summary>
        public string kscnyl { get; set; }
        /// <summary>
        ///增压储能压力
        ///</summary>
        public string zycnyl { get; set; }
        /// <summary>
        ///回锤缓冲位置
        ///</summary>
        public string hchcwz { get; set; }
        /// <summary>
        ///开模终止位置
        ///</summary>
        public string kmzzwz { get; set; }
        /// <summary>
        ///模具位置
        ///</summary>
        public string mjwz { get; set; }
        /// <summary>
        ///油温
        ///</summary>
        public string yw { get; set; }
        /// <summary>
        ///慢速位置
        ///</summary>
        public string mswz { get; set; }
        /// <summary>
        ///模具填充结束
        ///</summary>
        public string mjtcjs { get; set; }
        /// <summary>
        ///压缩行程
        ///</summary>
        public string yasxc { get; set; }
        /// <summary>
        ///浇铸活塞
        ///</summary>
        public string jzhs { get; set; }
        /// <summary>
        ///总回路数
        ///</summary>
        public string zhls { get; set; }
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
        public string sysj { get; set; }
        /// <summary>
        ///热模件次数
        ///</summary>
        public string rmjcs { get; set; }

    }

    public class sjcj_mapper : ClassMapper<sjcj>
    {
        public sjcj_mapper()
        {
            AutoMap();
        }
    }
}
