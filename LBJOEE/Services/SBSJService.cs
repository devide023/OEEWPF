using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LBJOEE.Models;
using Dapper;
using System.Data;
using LBJOEE.Tools;
using log4net;
using DapperExtensions;
using DapperExtensions.Predicate;
using Oracle.ManagedDataAccess.Client;

namespace LBJOEE.Services
{
    /// <summary>
    /// 设备数据服务类
    /// </summary>
    public class SBSJService : DBImp<sjcjnew>
    {
        //private static SBSJService instance = null;
        //private static readonly object padlock = new object();
        private ILog log = null;
        public SBSJService()
        {
            log = LogManager.GetLogger(this.GetType());
        }

        //public static SBSJService Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (instance == null)
        //                {
        //                    instance = new SBSJService();
        //                }
        //            }
        //        }
        //        return instance;
        //    }
        //}

        public void SaveOriginalData(originaldata entity)
        {
            try
            {
               
                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into receivedata ");
                    sql.Append(" (rq, sbbh, ip, json) ");
                    sql.Append(" values ");
                    sql.Append(" (sysdate, :sbbh, :ip, :json) ");
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":sbbh", entity.sbbh, DbType.String, ParameterDirection.Input);
                    p.Add(":ip", entity.ip, DbType.String, ParameterDirection.Input);
                    p.Add(":json", entity.json, DbType.String, ParameterDirection.Input);
                    if (Tool.IsPing())
                    {
                        db.Execute(sql.ToString(), p);
                    }
                    else
                    {
                        DataBackUp.SaveOrginalDataToLocal(entity);
                    }
                
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace + e.Message);
                //Environment.Exit(0);
            }
        }
        /// <summary>
        /// 保存掉网时的数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddByDate(sjcjnew entity)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into sjcj ");
                    sql.Append("(cjsj, sbbh, sbip, jp, jgs, yssd, jysj, jssj, lbhd, zyyl, yxzt, bjzt, xhsj, sdzt, zdzt, jt, ljkjsj, dzcdqwz, dqyl, dqll, ksyl, essd, sssd, sdqd, tqxc, tcsj, lbwz, gswz, mysd, gssd, zzyl, ysyl, ysll, zltx, hml, jzsyhd, ysxc, xqctsj, zzysj, sscnylsjz, zycnylsjz, smqdyl, smqdll, smqdwz, smksyl, smksll, smkswz, skdyyl, skdyll, smdywz, smgyyl, smgyll, smgywz, kssmsd, mssmsd, kmhcyl, kmhcll, kmhcwz, kmkyyl, kmksll, kmkswz, kmgyyl, kmgyll, kmgywz, mskmsd, kskmsd, cx1jryl, cx1jrll, cx1htyl, cx1htll, cx2jryl, cx2jrll, cx2htyl, cx2htll, cxrys, cxcys, cxcndqyl, cxcnyltlz, cxcnylsdz, kyyswz, kyeswz, kysswz, kyzywz, kygcwz, dqylsd, dqllsd, dqyssd, dhylsd, dhllsd, dhyssd, dzcs, kscnyl, zycnyl, hchcwz, kmzzwz, mjwz, yw, mswz, mjtcjs, yasxc, jzhs, zhls, gsks, gsqj, sysj, rmjcs) ");
                    sql.Append(" values ");
                    sql.Append(" (:cjsj, :sbbh, :sbip, :jp, :jgs, :yssd, :jysj, :jssj, :lbhd, :zyyl,  :yxzt, :bjzt, :xhsj, :sdzt, :zdzt, :jt, :ljkjsj, :dzcdqwz, :dqyl, :dqll, :ksyl, :essd, :sssd, :sdqd, :tqxc, :tcsj, :lbwz, :gswz, :mysd, :gssd, :zzyl, :ysyl, :ysll, :zltx, :hml, :jzsyhd, :ysxc, :xqctsj, :zzysj, :sscnylsjz, :zycnylsjz, :smqdyl, :smqdll, :smqdwz, :smksyl, :smksll, :smkswz, :skdyyl, :skdyll, :smdywz, :smgyyl, :smgyll, :smgywz, :kssmsd, :mssmsd, :kmhcyl, :kmhcll, :kmhcwz, :kmkyyl, :kmksll, :kmkswz, :kmgyyl, :kmgyll, :kmgywz, :mskmsd, :kskmsd, :cx1jryl, :cx1jrll, :cx1htyl, :cx1htll, :cx2jryl, :cx2jrll, :cx2htyl, :cx2htll, :cxrys, :cxcys, :cxcndqyl, :cxcnyltlz, :cxcnylsdz, :kyyswz, :kyeswz, :kysswz, :kyzywz, :kygcwz, :dqylsd, :dqllsd, :dqyssd, :dhylsd, :dhllsd, :dhyssd, :dzcs, :kscnyl, :zycnyl, :hchcwz, :kmzzwz, :mjwz, :yw, :mswz, :mjtcjs, :yasxc, :jzhs, :zhls, :gsks, :gsqj, :sysj, :rmjcs)");
                    return db.Execute(sql.ToString(), entity);

                
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return 0;
            }
        }

        public int TJSJCJ(sjcjnew entity)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into tjsjcj ");
                    sql.Append("(cjsj, sbbh, sbip, jp, jgs, yssd, jysj, jssj, lbhd, zyyl, yxzt, bjzt, xhsj, sdzt, zdzt, jt, ljkjsj, dzcdqwz, dqyl, dqll, ksyl, essd, sssd, sdqd, tqxc, tcsj, lbwz, gswz, mysd, gssd, zzyl, ysyl, ysll, zltx, hml, jzsyhd, ysxc, xqctsj, zzysj, sscnylsjz, zycnylsjz, smqdyl, smqdll, smqdwz, smksyl, smksll, smkswz, skdyyl, skdyll, smdywz, smgyyl, smgyll, smgywz, kssmsd, mssmsd, kmhcyl, kmhcll, kmhcwz, kmkyyl, kmksll, kmkswz, kmgyyl, kmgyll, kmgywz, mskmsd, kskmsd, cx1jryl, cx1jrll, cx1htyl, cx1htll, cx2jryl, cx2jrll, cx2htyl, cx2htll, cxrys, cxcys, cxcndqyl, cxcnyltlz, cxcnylsdz, kyyswz, kyeswz, kysswz, kyzywz, kygcwz, dqylsd, dqllsd, dqyssd, dhylsd, dhllsd, dhyssd, dzcs, kscnyl, zycnyl, hchcwz, kmzzwz, mjwz, yw, mswz, mjtcjs, yasxc, jzhs, zhls, gsks, gsqj, sysj, rmjcs) ");
                    sql.Append(" values ");
                    sql.Append(" (:cjsj, :sbbh, :sbip, :jp, :jgs, :yssd, :jysj, :jssj, :lbhd, :zyyl,  :yxzt, :bjzt, :xhsj, :sdzt, :zdzt, :jt, :ljkjsj, :dzcdqwz, :dqyl, :dqll, :ksyl, :essd, :sssd, :sdqd, :tqxc, :tcsj, :lbwz, :gswz, :mysd, :gssd, :zzyl, :ysyl, :ysll, :zltx, :hml, :jzsyhd, :ysxc, :xqctsj, :zzysj, :sscnylsjz, :zycnylsjz, :smqdyl, :smqdll, :smqdwz, :smksyl, :smksll, :smkswz, :skdyyl, :skdyll, :smdywz, :smgyyl, :smgyll, :smgywz, :kssmsd, :mssmsd, :kmhcyl, :kmhcll, :kmhcwz, :kmkyyl, :kmksll, :kmkswz, :kmgyyl, :kmgyll, :kmgywz, :mskmsd, :kskmsd, :cx1jryl, :cx1jrll, :cx1htyl, :cx1htll, :cx2jryl, :cx2jrll, :cx2htyl, :cx2htll, :cxrys, :cxcys, :cxcndqyl, :cxcnyltlz, :cxcnylsdz, :kyyswz, :kyeswz, :kysswz, :kyzywz, :kygcwz, :dqylsd, :dqllsd, :dqyssd, :dhylsd, :dhllsd, :dhyssd, :dzcs, :kscnyl, :zycnyl, :hchcwz, :kmzzwz, :mjwz, :yw, :mswz, :mjtcjs, :yasxc, :jzhs, :zhls, :gsks, :gsqj, :sysj, :rmjcs)");
                    return db.Execute(sql.ToString(), entity);
                
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return 0;
            }
        }
        public int TJSJCJ_ServerDate(sjcjnew entity)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append(" declare \n");
                    sql.Append(" vserverdate date;\n");
                    sql.Append(" begin \n");
                    sql.Append(" select sysdate into vserverdate from dual;\n");
                    sql.Append(" insert into tjsjcj ");
                    sql.Append(" (cjsj, sbbh, sbip, jp, jgs, yssd, jysj, jssj, lbhd, zyyl, yxzt, bjzt, xhsj, sdzt, zdzt, jt, ljkjsj, dzcdqwz, dqyl, dqll, ksyl, essd, sssd, sdqd, tqxc, tcsj, lbwz, gswz, mysd, gssd, zzyl, ysyl, ysll, zltx, hml, jzsyhd, ysxc, xqctsj, zzysj, sscnylsjz, zycnylsjz, smqdyl, smqdll, smqdwz, smksyl, smksll, smkswz, skdyyl, skdyll, smdywz, smgyyl, smgyll, smgywz, kssmsd, mssmsd, kmhcyl, kmhcll, kmhcwz, kmkyyl, kmksll, kmkswz, kmgyyl, kmgyll, kmgywz, mskmsd, kskmsd, cx1jryl, cx1jrll, cx1htyl, cx1htll, cx2jryl, cx2jrll, cx2htyl, cx2htll, cxrys, cxcys, cxcndqyl, cxcnyltlz, cxcnylsdz, kyyswz, kyeswz, kysswz, kyzywz, kygcwz, dqylsd, dqllsd, dqyssd, dhylsd, dhllsd, dhyssd, dzcs, kscnyl, zycnyl, hchcwz, kmzzwz, mjwz, yw, mswz, mjtcjs, yasxc, jzhs, zhls, gsks, gsqj, sysj, rmjcs) ");
                    sql.Append(" values ");
                    sql.Append(" (vserverdate, :sbbh, :sbip, :jp, :jgs, :yssd, :jysj, :jssj, :lbhd, :zyyl,  :yxzt, :bjzt, :xhsj, :sdzt, :zdzt, :jt, :ljkjsj, :dzcdqwz, :dqyl, :dqll, :ksyl, :essd, :sssd, :sdqd, :tqxc, :tcsj, :lbwz, :gswz, :mysd, :gssd, :zzyl, :ysyl, :ysll, :zltx, :hml, :jzsyhd, :ysxc, :xqctsj, :zzysj, :sscnylsjz, :zycnylsjz, :smqdyl, :smqdll, :smqdwz, :smksyl, :smksll, :smkswz, :skdyyl, :skdyll, :smdywz, :smgyyl, :smgyll, :smgywz, :kssmsd, :mssmsd, :kmhcyl, :kmhcll, :kmhcwz, :kmkyyl, :kmksll, :kmkswz, :kmgyyl, :kmgyll, :kmgywz, :mskmsd, :kskmsd, :cx1jryl, :cx1jrll, :cx1htyl, :cx1htll, :cx2jryl, :cx2jrll, :cx2htyl, :cx2htll, :cxrys, :cxcys, :cxcndqyl, :cxcnyltlz, :cxcnylsdz, :kyyswz, :kyeswz, :kysswz, :kyzywz, :kygcwz, :dqylsd, :dqllsd, :dqyssd, :dhylsd, :dhllsd, :dhyssd, :dzcs, :kscnyl, :zycnyl, :hchcwz, :kmzzwz, :mjwz, :yw, :mswz, :mjtcjs, :yasxc, :jzhs, :zhls, :gsks, :gsqj, :sysj, :rmjcs);\n");
                    sql.Append(" commit;\n");
                    sql.Append(" end;");
                    return db.Execute(sql.ToString(), entity);

                
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return 0;
            }
        }
        /// <summary>
        /// 查询相应时间内，非运行状态下上传数据条目
        /// </summary>
        /// <param name="sbbh"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int Get_NoRunQty(string sbbh, int interval = 7)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select count(sbbh) qty ");
                    sql.Append(" FROM   tjsjcj ");
                    sql.Append(" where  cjsj between sysdate - :interval / (24 * 60) and sysdate ");
                    sql.Append(" and    sbbh = :sbbh");
                    return db.ExecuteScalar<int>(sql.ToString(), new { interval = interval, sbbh = sbbh });
                
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return 0;
            }
        }

        /// <summary>
        /// 获取非运行状态下，上传的数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<sjcjnew> Get_NoRunList(string sbbh, int interval = 7, DateTime tjkssj = default(DateTime))
        {
            try
            {
                
                    if (tjkssj == default(DateTime))
                    {
                        tjkssj = db.ExecuteScalar<DateTime>("select sysdate from dual");
                    }
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select count(cjsj) as cnt ");
                    sql.Append(" FROM   tjsjcj ");
                    sql.Append($" where  cjsj between sysdate - (1 / (24*60)) * {interval} and sysdate ");
                    sql.Append(" and    sbbh = :sbbh ");
                    StringBuilder tsql = new StringBuilder();
                    tsql.Append("select rowid as rid,cjsj, sbbh, sbip, jp, jgs, yssd, jysj, jssj, lbhd, zyyl, yxzt, bjzt, xhsj, sdzt, zdzt, jt, ljkjsj, dzcdqwz, dqyl, dqll, ksyl, essd, sssd, sdqd, tqxc, tcsj, lbwz, gswz, mysd, gssd, zzyl, ysyl, ysll, zltx, hml, jzsyhd, ysxc, xqctsj, zzysj, sscnylsjz, zycnylsjz, smqdyl, smqdll, smqdwz, smksyl, smksll, smkswz, skdyyl, skdyll, smdywz, smgyyl, smgyll, smgywz, kssmsd, mssmsd, kmhcyl, kmhcll, kmhcwz, kmkyyl, kmksll, kmkswz, kmgyyl, kmgyll, kmgywz, mskmsd, kskmsd, cx1jryl, cx1jrll, cx1htyl, cx1htll, cx2jryl, cx2jrll, cx2htyl, cx2htll, cxrys, cxcys, cxcndqyl, cxcnyltlz, cxcnylsdz, kyyswz, kyeswz, kysswz, kyzywz, kygcwz, dqylsd, dqllsd, dqyssd, dhylsd, dhllsd, dhyssd, dzcs, kscnyl, zycnyl, hchcwz, kmzzwz, mjwz, yw, mswz, mjtcjs, yasxc, jzhs, zhls, gsks, gsqj, sysj, rmjcs ");
                    tsql.Append(" FROM   tjsjcj ");
                    tsql.Append(" where  cjsj >= :tjkssj ");
                    tsql.Append(" and    sbbh = :sbbh ");
                    //var cnt = db.ExecuteScalar<int>(sql.ToString(), new { sbbh = sbbh, interval = interval });
                    //log.Info($"停机时数采条数：{cnt}");
                    //if (cnt > 0)
                    //{
                    var jgs = db.ExecuteScalar<long>("select nvl(max(jgs),0) FROM sjcj where sbbh = :sbbh and trunc(cjsj) = trunc(:tjkssj) ", new { sbbh = sbbh, tjkssj = tjkssj });
                    var minjgs = db.ExecuteScalar<long>($"select nvl(min(jgs),0) FROM tjsjcj where sbbh = :sbbh and cjsj between :tjkssj and sysdate ", new { sbbh = sbbh, tjkssj = tjkssj });
                    var jgs_cz = jgs - minjgs;
                    log.Info($"停机加工数最小值:{minjgs}与采集加工数最大值:{jgs}的差值：{jgs_cz}");
                    if (Math.Abs(jgs_cz) < 100)
                    {
                        var norunlist = Get_NoRunListByJGS(sbbh, jgs, tjkssj);
                        return norunlist;
                    }
                    else
                    {
                        var listtjcj = db.Query<sjcjnew>(tsql.ToString(), new { sbbh = sbbh, tjkssj = tjkssj });
                        return listtjcj;
                    }
                    //}
                    //else
                    //{
                    //    return new List<sjcjnew>();
                    //}

                
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return new List<sjcjnew>();
            }
        }
        /// <summary>
        /// 通过加工数字段取数停机状态数据
        /// </summary>
        /// <param name="sbbh"></param>
        /// <returns></returns>
        public IEnumerable<sjcjnew> Get_NoRunListByJGS(string sbbh, long jgs, DateTime tjkssj)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select rowid as rid,cjsj, sbbh, sbip, jp, jgs, yssd, jysj, jssj, lbhd, zyyl, yxzt, bjzt, xhsj, sdzt, zdzt, jt, ljkjsj, dzcdqwz, dqyl, dqll, ksyl, essd, sssd, sdqd, tqxc, tcsj, lbwz, gswz, mysd, gssd, zzyl, ysyl, ysll, zltx, hml, jzsyhd, ysxc, xqctsj, zzysj, sscnylsjz, zycnylsjz, smqdyl, smqdll, smqdwz, smksyl, smksll, smkswz, skdyyl, skdyll, smdywz, smgyyl, smgyll, smgywz, kssmsd, mssmsd, kmhcyl, kmhcll, kmhcwz, kmkyyl, kmksll, kmkswz, kmgyyl, kmgyll, kmgywz, mskmsd, kskmsd, cx1jryl, cx1jrll, cx1htyl, cx1htll, cx2jryl, cx2jrll, cx2htyl, cx2htll, cxrys, cxcys, cxcndqyl, cxcnyltlz, cxcnylsdz, kyyswz, kyeswz, kysswz, kyzywz, kygcwz, dqylsd, dqllsd, dqyssd, dhylsd, dhllsd, dhyssd, dzcs, kscnyl, zycnyl, hchcwz, kmzzwz, mjwz, yw, mswz, mjtcjs, yasxc, jzhs, zhls, gsks, gsqj, sysj, rmjcs ");
                    sql.Append(" FROM   tjsjcj ");
                    sql.Append(" where  jgs > :jgs ");
                    sql.Append(" and cjsj >= :tjkssj ");
                    sql.Append(" and    sbbh = :sbbh ");
                    return db.Query<sjcjnew>(sql.ToString(), new { sbbh = sbbh, jgs = jgs, tjkssj = tjkssj });
                
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return new List<sjcjnew>();
            }
        }

        public bool DelTjsjcj(string rid)
        {
            try
            {
                
                    var cnt = db.Execute("delete from tjsjcj where rowid = :rid ", new { rid = rid });
                    return cnt > 0 ? true : false;
                
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return false;
            }
        }

        public IEnumerable<base_sbxx_conf> Get_Conf_BySBBH(string sbbh)
        {
            try
            {
                return Db.GetList<base_sbxx_conf>(Predicates.Field<base_sbxx_conf>(f => f.sbbh, Operator.Eq, sbbh));
                //using (var db = Db.Connection)
                //{
                //    if(db.State == ConnectionState.Closed)
                //    {
                //        db.Open();
                //    }
                //    StringBuilder sql = new StringBuilder();
                //    sql.Append("select sbbh,confkey,confval FROM  base_sbxx_conf where sbbh=:sbbh ");
                //    return db.Query<base_sbxx_conf>(sql.ToString(), new { sbbh = sbbh });
                //}
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return new List<base_sbxx_conf>();
            }
        }
    }
}
