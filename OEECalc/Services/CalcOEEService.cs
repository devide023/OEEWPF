using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using DapperExtensions.Predicate;
using OEECalc.Model;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    public class CalcOEEService : OracleBaseFixture
    {
        //private static CalcOEEService instance = null;
        //private static readonly object padlock = new object();
        private ILog log;
        private LBJMesService _messervice = null;
        private SBXXService _sbxxservice = null;
        private static readonly Lazy<CalcOEEService> lazy = new Lazy<CalcOEEService>(() => new CalcOEEService());
        private CalcOEEService()
        {
            _messervice = LBJMesService.Instance;
            _sbxxservice = SBXXService.Instance;
            log = LogManager.GetLogger(this.GetType());
        }
        public static CalcOEEService Instance { get { return lazy.Value; } }
        //public static CalcOEEService Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (instance == null)
        //                {
        //                    instance = new CalcOEEService();
        //                }
        //            }
        //        }
        //        return instance;
        //    }
        //}

        private IEnumerable<base_sbxx> Get_SBBHList()
        {
            try
            {
                
                    string sql = "select sbbh,sbqy from base_sbxx where scbz='N' order by sbqy ";
                    return db.Query<base_sbxx>(sql);
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sbxx>();
            }
        }

        /// <summary>
        /// 统计设备停机时长（单位：秒）
        /// </summary>
        /// <param name="sbbh"></param>
        /// <param name="ksrq"></param>
        /// <param name="jsrq"></param>
        /// <returns></returns>
        public IEnumerable<sbtj> GetTJInfo(string sbbh, DateTime ksrq, DateTime jsrq)
        {
            try
            {
               
                    StringBuilder sql1 = new StringBuilder();
                    sql1.Append("select sbbh, sbqy, sbzt, sfjx, sfhm, sfgz, sfql, sfqttj, sfxm, sfts,sfby,sflgtj, jxkssj, hmkssj, gzkssj, qlkssj, qttjkssj, xmkssj, tskssj,bytjkssj,lgtjkssj ");
                    sql1.Append(" from base_sbxx ");
                    sql1.Append(" where sbbh = :sbbh ");
                    var q = db.Query<base_sbxx>(sql1.ToString(), new { sbbh = sbbh });
                    base_sbxx sbxx_entity = new base_sbxx();
                    if (q.Count() > 0)
                    {
                        sbxx_entity = q.First();
                    }
                    List<sbtj> retlist = new List<sbtj>();
                    //只点开始停机，未点结束停机情况
                    retlist = Tool.TimeTool.Calc_SBTjSD(sbxx_entity).Where(t => t.tjkssj >= ksrq && t.tjkssj <= jsrq && t.tjjssj >= ksrq && t.tjjssj <= jsrq).ToList();
                    //完整停机数据
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select id, sbbh, tjlx, tjsj, tjkssj, tjjssj, tjms from SBTJ where lx = '0' and sbbh = :sbbh and tjjssj between :rq1 and :rq2 ");
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    p.Add(":rq1", ksrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    p.Add(":rq2", jsrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    var list = db.Query<sbtj>(sql.ToString(), p);
                    foreach (var item in list)
                    {
                        retlist.AddRange(Tool.TimeTool.Calc_SBTJSD(item).Where(t => t.tjkssj >= ksrq && t.tjkssj <= jsrq && t.tjjssj >= ksrq && t.tjjssj <= jsrq));
                    }
                    return retlist;
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<sbtj>();
            }
        }
        public IEnumerable<sbtj> GetTJInfoHis(string sbbh, DateTime ksrq, DateTime jsrq)
        {
            try
            {
                
                    List<sbtj> retlist = new List<sbtj>();
                    //完整停机数据
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select id, sbbh, tjlx, tjsj, tjkssj, tjjssj, tjms from SBTJ where lx = '1' and sbbh = :sbbh and tjkssj between :rq1 and :rq2 and tjjssj between :rq1 and :rq2 ");
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    p.Add(":rq1", ksrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    p.Add(":rq2", jsrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    retlist = db.Query<sbtj>(sql.ToString(), p).ToList();
                    return retlist;
                
            }
            catch (Exception)
            {

                throw;
            }
        }
        public decimal SBJP(string sbbh, DateTime ksrq, DateTime jsrq)
        {
            try
            {
                
                    string sql = "select f_sjjp(:ksrq, :jsrq, :sbbh) as jp FROM dual";
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":ksrq", ksrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    p.Add(":jsrq", jsrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                    p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    var jp = db.ExecuteScalar<decimal>(sql, p);
                    return jp;
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0m;
            }
        }
        /// <summary>
        /// 获取压射次数
        /// </summary>
        /// <param name="sbbh"></param>
        /// <param name="ksrq"></param>
        /// <param name="jsrq"></param>
        /// <returns></returns>
        public long GetYscs(string sbbh, DateTime ksrq, DateTime jsrq)
        {
            try
            {
                
                    var jgstjfs = "1";
                    var jgsconf = Tool.ConfigTool.Read_JGSTJ_Conf().Where(t => t.sbbh == sbbh);
                    if (jgsconf.Count() > 0)
                    {
                        jgstjfs = jgsconf.Select(t => t.jgstj).FirstOrDefault().ToString();
                    }
                    if (jgstjfs == "1")
                    {
                        DynamicParameters p = new DynamicParameters();
                        p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                        p.Add(":ksrq", ksrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                        p.Add(":jsrq", jsrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                        string maxsql = "select max(jgs) FROM sjcj where sbbh = :sbbh and cjsj between :ksrq and :jsrq and jgs<> 0 ";
                        string minsql = "select min(jgs) FROM sjcj where sbbh = :sbbh and cjsj between :ksrq and :jsrq and jgs<> 0";
                        var max = db.ExecuteScalar<long>(maxsql, p);
                        var min = db.ExecuteScalar<long>(minsql, p);
                        return max - min;
                    }
                    else
                    {
                        DynamicParameters p = new DynamicParameters();
                        p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                        p.Add(":ksrq", ksrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                        p.Add(":jsrq", jsrq, System.Data.DbType.DateTime, System.Data.ParameterDirection.Input);
                        return db.ExecuteScalar<long>("select count(jgs) FROM sjcj where sbbh = :sbbh and cjsj between :ksrq and :jsrq and jgs<> 0", p);
                    }
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0;
            }
        }
        /// <summary>
        /// 获取计划停机时间
        /// </summary>
        /// <returns></returns>
        public IEnumerable<sbjhtjsj> GetJhTJSJ()
        {
            try
            {
               
                    string sql = "select sbbh, sbqy, tjsj from sbjhtjsj";
                    return db.Query<sbjhtjsj>(sql);
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<sbjhtjsj>();
            }
        }

        public void SaveOEE(DateTime rq)
        {
            try
            {
                
                    PredicateGroup pg = new PredicateGroup()
                    {
                        Operator = GroupOperator.And,
                        Predicates = new List<IPredicate>()
                    };
                    var sblist = Get_SBBHList();
                    var jhtjlist = GetJhTJSJ();
                    var bckssj = DateTime.Now;
                    var bcjssj = DateTime.Now;
                    var bc = string.Empty;
                    //白班
                    var d0 = Convert.ToDateTime(rq.AddDays(-1).ToString("yyyy-MM-dd 20:00:00"));
                    var d1 = Convert.ToDateTime(rq.ToString("yyyy-MM-dd 08:00:00"));
                    //晚班
                    var d2 = Convert.ToDateTime(rq.ToString("yyyy-MM-dd 20:00:00"));
                    var d3 = Convert.ToDateTime(rq.AddDays(1).ToString("yyyy-MM-dd 08:00:00"));
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            bckssj = d1;
                            bcjssj = d2;
                            bc = "白班";
                        }
                        if (i == 1)
                        {
                            bckssj = d2;
                            bcjssj = d3;
                            bc = "夜班";
                        }
                        foreach (var item in sblist)
                        {
                            //停机信息
                            var tjlist = GetTJInfoHis(item.sbbh, bckssj, bcjssj);
                            //装配计划
                            var zpjhlist = _messervice.Get_PlanInfo(item.sbqy, bckssj, bcjssj);
                            //节拍
                            var jp = SBJP(item.sbbh, bckssj, bcjssj);
                            //压射次数
                            var yscs = GetYscs(item.sbbh, bckssj, bcjssj);
                            var jxsj = 0m;
                            var tssj = 0m;
                            var xjsj = 0m;
                            var xmsj = 0m;
                            var hmsj = 0m;
                            var dlsj = 0m;
                            var qtsj = 0m;
                            var bysj = 0m;
                            var lgsj = 0m;
                            decimal fjhtjsj = 0m;//非计划停机时间
                            if (tjlist.Count() > 0)
                            {
                                jxsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("检修")).Sum(t => t.tjsj));
                                tssj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("调试")).Sum(t => t.tjsj));
                                xjsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("修机")).Sum(t => t.tjsj));
                                xmsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("修模")).Sum(t => t.tjsj));
                                dlsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("待料")).Sum(t => t.tjsj));
                                qtsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("计划")).Sum(t => t.tjsj));
                                hmsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("换模")).Sum(t => t.tjsj));
                                bysj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("保养")).Sum(t => t.tjsj));
                                lgsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("离岗")).Sum(t => t.tjsj));
                                fjhtjsj = Convert.ToDecimal(jxsj + tssj + xjsj + xmsj + dlsj + qtsj + hmsj + bysj + lgsj);
                            }
                            decimal jhtjsj = 0m; //计划停机时间,休息时间等
                            if (jhtjlist.Count() > 0)
                            {
                                jhtjsj = Convert.ToDecimal(jhtjlist.Where(t => t.sbbh == item.sbbh).Sum(t => t.tjsj));
                            }
                            //下线数量
                            var xxsl = 0;
                            //计划数量
                            var jhsl = 0;
                            var bhgpsl = 0;
                            string wlbm = string.Empty;
                            string wlmc = string.Empty;
                            string yzmh = string.Empty;//压铸模号
                            if (zpjhlist.Count() > 0)
                            {
                                jhsl = zpjhlist.Sum(t => t.scsl);
                                xxsl = zpjhlist.Sum(t => t.xxcnt);
                                wlbm = zpjhlist.First().wlbm;
                                wlmc = zpjhlist.First().wlmc;
                                yzmh = zpjhlist.First().zmould;
                            }
                            //非计划停机时间
                            var fjhtjfzs = Math.Round(Convert.ToDecimal(fjhtjsj / 60m), 5);
                            //实际运行时间=工作时间-计划停机时间-非计划停机时间
                            var s = 720m - Convert.ToDecimal(jhtjsj) - fjhtjfzs;
                            //时间利用率
                            decimal sjlyl = Math.Round(s / 720m, 5);
                            //合格率
                            decimal hgl = 0m;
                            if (yscs != 0)
                            {
                                hgl = Math.Round(Convert.ToDecimal(xxsl) / Convert.ToDecimal(yscs), 5);
                            }
                            //性能稼动率(表现指数)
                            decimal bxzs = 0;
                            if (jp != 0 && s != 0)
                            {
                                bxzs = Math.Round(xxsl / ((s * 60) / jp), 5);
                            }
                            sboee oee = new sboee();
                            oee.sbbh = item.sbbh;
                            oee.sbqy = item.sbqy;
                            oee.rq = bckssj;
                            oee.bc = bc;
                            oee.jxsj = jxsj > 0 ? Math.Round(jxsj / 60m, 4) : 0m;
                            oee.tssj = tssj > 0 ? Math.Round(tssj / 60m, 4) : 0m;
                            oee.xmsj = xmsj > 0 ? Math.Round(xmsj / 60m, 4) : 0m;
                            oee.xjsj = xjsj > 0 ? Math.Round(xjsj / 60m, 4) : 0m;
                            oee.hmsj = hmsj > 0 ? Math.Round(hmsj / 60m, 4) : 0m;
                            oee.dlsj = dlsj > 0 ? Math.Round(dlsj / 60m, 4) : 0m;
                            oee.qtsj = qtsj > 0 ? Math.Round(qtsj / 60m, 4) : 0m;
                            oee.bysj = bysj > 0 ? Math.Round(bysj / 60m, 4) : 0m;
                            oee.lgsj = lgsj > 0 ? Math.Round(lgsj / 60m, 4) : 0m;
                            oee.xxsj = jhtjsj;
                            oee.xxsl = xxsl;//下线数量
                            oee.jhsl = jhsl;//计划数量
                            oee.wlbm = wlbm;
                            oee.wlmc = wlmc;
                            oee.mjbh = yzmh;
                            oee.jp = jp;//节拍
                            oee.hgpsl = xxsl;
                            oee.bhgpsl = bhgpsl;
                            oee.yscs = yscs;//压射次数
                            oee.gzsj = 720m;//工作时间
                            oee.yxsj = oee.gzsj - fjhtjfzs - oee.xxsj;
                            oee.lyl = sjlyl;
                            oee.hgl = hgl;
                            oee.bxzs = bxzs;
                            oee.oee = Math.Round(sjlyl * hgl * bxzs, 5);
                            var hasitem = db.ExecuteScalar<int>("select count(rq) cnt from sboee where rq=:rq and sbbh=:sbbh", new { rq = bckssj, sbbh = item.sbbh });
                            if (hasitem == 0)
                            {
                                Db.Insert<sboee>(oee);
                            }
                            else
                            {
                                pg.Predicates.Clear();
                                pg.Predicates.Add(Predicates.Field<sboee>(f => f.rq, Operator.Eq, bckssj));
                                pg.Predicates.Add(Predicates.Field<sboee>(f => f.sbbh, Operator.Eq, item.sbbh));
                                var finditem = Db.GetList<sboee>(pg).FirstOrDefault();
                                if (finditem != null)
                                {
                                    var id = finditem.id;
                                    finditem = oee;
                                    finditem.id = id;
                                    Db.Update<sboee>(finditem);
                                }
                            }
                        }
                    }
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        public void SaveOEE()
        {
            try
            {
                
                    PredicateGroup pg = new PredicateGroup()
                    {
                        Operator = GroupOperator.And,
                        Predicates = new List<IPredicate>()
                    };
                    var sblist = Get_SBBHList();
                    var jhtjlist = GetJhTJSJ();
                    var bcxx = Tool.TimeTool.GetBCInfo(DateTime.Now);
                    var bckssj = DateTime.Now;
                    var bcjssj = DateTime.Now;
                    var bc = string.Empty;
                    bckssj = bcxx.up_kssj;
                    bcjssj = bcxx.up_jssj;
                    bc = bcxx.up_bcmc;
                    foreach (var item in sblist)
                    {
                        //停机信息
                        var tjlist = GetTJInfo(item.sbbh, bckssj, bcjssj);
                        //装配计划
                        var zpjhlist = _messervice.Get_PlanInfo(item.sbqy, bckssj, bcjssj);
                        //节拍
                        var jp = SBJP(item.sbbh, bckssj, bcjssj);
                        //压射次数
                        var yscs = GetYscs(item.sbbh, bckssj, bcjssj);
                        var jxsj = 0m;
                        var tssj = 0m;
                        var xjsj = 0m;
                        var xmsj = 0m;
                        var hmsj = 0m;
                        var dlsj = 0m;
                        var qtsj = 0m;
                        var bysj = 0m;
                        var lgsj = 0m;
                        decimal fjhtjsj = 0m;//非计划停机时间
                        if (tjlist.Count() > 0)
                        {
                            jxsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("检修")).Sum(t => t.tjsj));
                            tssj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("调试")).Sum(t => t.tjsj));
                            xjsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("修机")).Sum(t => t.tjsj));
                            xmsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("修模")).Sum(t => t.tjsj));
                            dlsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("待料")).Sum(t => t.tjsj));
                            qtsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("计划")).Sum(t => t.tjsj));
                            hmsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("换模")).Sum(t => t.tjsj));
                            bysj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("保养")).Sum(t => t.tjsj));
                            lgsj = Convert.ToDecimal(tjlist.Where(t => t.tjlx.Contains("离岗")).Sum(t => t.tjsj));
                            fjhtjsj = Convert.ToDecimal(jxsj + tssj + xjsj + xmsj + dlsj + qtsj + hmsj + bysj + lgsj);
                        }
                        decimal jhtjsj = 0m; //计划停机时间,休息时间等
                        if (jhtjlist.Count() > 0)
                        {
                            jhtjsj = Convert.ToDecimal(jhtjlist.Where(t => t.sbbh == item.sbbh).Sum(t => t.tjsj));
                        }
                        //下线数量
                        var xxsl = 0;
                        //计划数量
                        var jhsl = 0;
                        var bhgpsl = 0;
                        string wlbm = string.Empty;
                        string wlmc = string.Empty;
                        string yzmh = string.Empty;//压铸模号
                        if (zpjhlist.Count() > 0)
                        {
                            jhsl = zpjhlist.Sum(t => t.scsl);
                            xxsl = zpjhlist.Sum(t => t.xxcnt);
                            wlbm = zpjhlist.First().wlbm;
                            wlmc = zpjhlist.First().wlmc;
                            yzmh = zpjhlist.First().zmould;
                        }
                        //非计划停机时间
                        var fjhtjfzs = Math.Round(Convert.ToDecimal(fjhtjsj / 60m), 5);
                        //实际运行时间=工作时间-计划停机时间-非计划停机时间
                        var s = 720m - Convert.ToDecimal(jhtjsj) - fjhtjfzs;
                        //时间利用率
                        decimal sjlyl = Math.Round(s / 720m, 5);
                        //合格率
                        decimal hgl = 0m;
                        if (yscs != 0)
                        {
                            hgl = Math.Round(Convert.ToDecimal(xxsl) / Convert.ToDecimal(yscs), 5);
                        }
                        //性能稼动率(表现指数)
                        decimal bxzs = 0;
                        if (jp != 0 && s != 0)
                        {
                            bxzs = Math.Round(xxsl / ((s * 60) / jp), 5);
                        }
                        sboee oee = new sboee();
                        oee.sbbh = item.sbbh;
                        oee.sbqy = item.sbqy;
                        oee.rq = bckssj;
                        oee.bc = bc;
                        oee.jxsj = jxsj > 0 ? Math.Round(jxsj / 60m, 4) : 0m;
                        oee.tssj = tssj > 0 ? Math.Round(tssj / 60m, 4) : 0m;
                        oee.xmsj = xmsj > 0 ? Math.Round(xmsj / 60m, 4) : 0m;
                        oee.xjsj = xjsj > 0 ? Math.Round(xjsj / 60m, 4) : 0m;
                        oee.hmsj = hmsj > 0 ? Math.Round(hmsj / 60m, 4) : 0m;
                        oee.dlsj = dlsj > 0 ? Math.Round(dlsj / 60m, 4) : 0m;
                        oee.qtsj = qtsj > 0 ? Math.Round(qtsj / 60m, 4) : 0m;
                        oee.bysj = bysj > 0 ? Math.Round(bysj / 60m, 4) : 0m;
                        oee.lgsj = lgsj > 0 ? Math.Round(lgsj / 60m, 4) : 0m;
                        oee.xxsj = jhtjsj;
                        oee.xxsl = xxsl;//下线数量
                        oee.jhsl = jhsl;//计划数量
                        oee.wlbm = wlbm;
                        oee.wlmc = wlmc;
                        oee.mjbh = yzmh;
                        oee.jp = jp;//节拍
                        oee.hgpsl = xxsl;
                        oee.bhgpsl = bhgpsl;
                        oee.yscs = yscs;//压射次数
                        oee.gzsj = 720m;//工作时间
                        oee.yxsj = oee.gzsj - fjhtjfzs - oee.xxsj;
                        oee.lyl = sjlyl;
                        oee.hgl = hgl;
                        oee.bxzs = bxzs;
                        oee.oee = Math.Round(sjlyl * hgl * bxzs, 5);
                        var hasitem = db.ExecuteScalar<int>("select count(rq) cnt from sboee where rq=:rq and sbbh=:sbbh", new { rq = bckssj, sbbh = item.sbbh });
                        if (hasitem == 0)
                        {
                            Db.Insert<sboee>(oee);
                        }
                        else
                        {
                            pg.Predicates.Clear();
                            pg.Predicates.Add(Predicates.Field<sboee>(f => f.rq, Operator.Eq, bckssj));
                            pg.Predicates.Add(Predicates.Field<sboee>(f => f.sbbh, Operator.Eq, item.sbbh));
                            var finditem = Db.GetList<sboee>(pg).FirstOrDefault();
                            if (finditem != null)
                            {
                                var id = finditem.id;
                                finditem = oee;
                                finditem.id = id;
                                Db.Update<sboee>(finditem);
                            }
                        }
                    }
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
    }
}
