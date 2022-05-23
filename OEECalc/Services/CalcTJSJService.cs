using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using OEECalc.Model;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    /// <summary>
    /// 计算停机时间，针对停机开始时间跨多个班次情况
    /// </summary>
    public class CalcTJSJService : OracleBaseFixture
    {
        //private static CalcTJSJService instance = null;
        //private static readonly object padlock = new object();
        private static readonly Lazy<CalcTJSJService> lazy = new Lazy<CalcTJSJService>(() => new CalcTJSJService());
        private ILog log;
        private SBXXService sbxxservice;
        private CalcTJSJService()
        {
            log = LogManager.GetLogger(this.GetType());
            sbxxservice = SBXXService.Instance;
        }
        public static CalcTJSJService Instance { get { return lazy.Value; } }
        //public static CalcTJSJService Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (instance == null)
        //                {
        //                    instance = new CalcTJSJService();
        //                }
        //            }
        //        }
        //        return instance;
        //    }
        //}
        /// <summary>
        /// 只有开始停机时间，未结束停机时间
        /// </summary>
        public void CalcTJSJ()
        {
            try
            {
               
                    var sbxxlist = sbxxservice.Get_SBXX_List().OrderBy(t => t.sbbh);
                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into sbtj ");
                    sql.Append("(sbbh, tjlx, tjsj, tjkssj, tjjssj, tjms,lx) ");
                    sql.Append(" select ");
                    sql.Append(":sbbh, :tjlx, :tjsj, :tjkssj, :tjjssj, :tjms,:lx from dual where ");
                    sql.Append(" not exists (select id from SBTJ where sbbh=:sbbh and tjlx=:tjlx and tjkssj=:tjkssj and lx='1' ) ");
                    //查询停机时间
                    StringBuilder tsql = new StringBuilder();
                    tsql.Append(" select id, sbbh, tjlx, tjsj, tjkssj, tjjssj, tjms ");
                    tsql.Append(" FROM   sbtj ");
                    tsql.Append(" where  sbbh = :sbbh ");
                    tsql.Append(" and    tjjssj between :rq1 and :rq2 ");
                    tsql.Append(" and    lx = '0' ");
                    var bcinfo = Tool.TimeTool.GetBCInfo(DateTime.Now);
                    foreach (var item in sbxxlist)
                    {
                        var tjsjlist = Tool.TimeTool.Calc_SBTjSD(item).ToList();
                        foreach (var obj in tjsjlist)
                        {
                            if (obj.tjkssj.Year >= 2021 && obj.tjjssj.Year >= 2021)
                            {
                                db.Execute(sql.ToString(), obj);
                            }
                        }
                        var q = db.Query<sbtj>(tsql.ToString(), new { sbbh = item.sbbh, rq1 = bcinfo.kssj.AddHours(-24), rq2 = bcinfo.jssj });
                        //停机数据
                        foreach (var obj in q)
                        {
                            //停机明细
                            var bctjsj_list = Tool.TimeTool.Calc_SBTJSD(obj);
                            foreach (var o in bctjsj_list)
                            {
                                if (o.tjkssj.Year >= 2021 && o.tjjssj.Year >= 2021)
                                {
                                    db.Execute(sql.ToString(), o);
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
        /// <summary>
        /// 检查停机时间段是否已存在
        /// </summary>
        /// <param name="kssj"></param>
        /// <param name="jssj"></param>
        /// <returns>存在返回true,不存在返回false</returns>
        private bool IsTJSJCZ(sbtj entity)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select id, sbbh, tjlx, tjsj, tjkssj, tjjssj, tjms, lx ");
                    sql.Append(" FROM   sbtj ");
                    sql.Append(" where  sbbh = :sbbh ");
                    sql.Append(" and lx = '1' ");
                    sql.Append(" and tjlx = :tjlx ");
                    sql.Append(" and    tjkssj = :tjkssj ");
                    var q = db.Query<sbtj>(sql.ToString(), new { sbbh = entity.sbbh, tjkssj = entity.tjkssj, tjlx = entity.tjlx });
                    if (q.Count() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }

        public void CheckCrossBC()
        {
            try
            {
               
                    StringBuilder sql = new StringBuilder();
                    sql.Append(" select * ");
                    sql.Append(" from SBTJ ");
                    sql.Append(" where lx is null ");
                    sql.Append(" and trunc(tjjssj) > to_date('2021-11-30', 'yyyy-mm-dd hh24:mi:ss') ");
                    var list = db.Query<sbtj>(sql.ToString());
                    foreach (var item in list)
                    {
                        var iscross = Tool.TimeTool.IsSameBC(item);
                        if (iscross)
                        {
                            db.Execute("update SBTJ set lx = :lx,gxsj=sysdate where id = :id ", new { lx = "1", id = item.id });
                        }
                        else
                        {
                            db.Execute("update SBTJ set lx = :lx,gxsj=sysdate where id = :id ", new { lx = "0", id = item.id });
                        }
                    }
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CreateTJMX()
        {
            try
            {
                

                    StringBuilder tsql = new StringBuilder();
                    tsql.Append(" select id, sbbh, tjlx, tjsj, tjkssj, tjjssj, tjms ");
                    tsql.Append(" FROM   sbtj ");
                    tsql.Append(" where   ");
                    tsql.Append(" lx = '0' ");

                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into sbtj ");
                    sql.Append("(sbbh, tjlx, tjsj, tjkssj, tjjssj, tjms,lx) ");
                    sql.Append(" select ");
                    sql.Append(":sbbh, :tjlx, :tjsj, :tjkssj, :tjjssj, :tjms,:lx from dual where ");
                    sql.Append(" not exists (select id from SBTJ where sbbh=:sbbh and tjlx=:tjlx and tjsj=:tjsj and tjkssj=:tjkssj and tjjssj = :tjjssj) ");

                    var q = db.Query<sbtj>(tsql.ToString());
                    //停机数据
                    foreach (var obj in q)
                    {
                        //停机明细
                        var bctjsj_list = Tool.TimeTool.Calc_SBTJSD(obj);
                        foreach (var o in bctjsj_list)
                        {
                            db.Execute(sql.ToString(), o);
                        }
                    }
                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
