using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using OEECalc.Model;

namespace OEECalc.Services
{
    /// <summary>
    /// 计算停机时间，针对停机开始时间跨多个班次情况
    /// </summary>
    public class CalcTJSJService : OracleBaseFixture
    {
        private static CalcTJSJService instance = null;
        private static readonly object padlock = new object();
        private ILog log;
        private SBXXService sbxxservice;
        private CalcTJSJService()
        {
            log = LogManager.GetLogger(this.GetType());
            sbxxservice = new SBXXService();
        }
        public static CalcTJSJService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new CalcTJSJService();
                        }
                    }
                }
                return instance;
            }
        }
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
                sql.Append(" not exists (select id from SBTJ where sbbh=:sbbh and tjlx=:tjlx and tjsj=:tjsj and tjkssj=:tjkssj and tjjssj = :tjjssj) ");
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
                    tjsjlist.ForEach(t => t.lx = "1");
                    foreach (var obj in tjsjlist)
                    {
                        Db.Connection.Execute(sql.ToString(), obj);
                    }
                    var q = Db.Connection.Query<sbtj>(tsql.ToString(), new { sbbh = item.sbbh, rq1 = bcinfo.kssj, rq2 = bcinfo.jssj });
                    //停机数据
                    foreach (var obj in q)
                    {
                        //停机明细
                        var bctjsj_list = Tool.TimeTool.Calc_SBTJSD(obj);
                        foreach (var o in bctjsj_list)
                        {
                            Db.Connection.Execute(sql.ToString(), o);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
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
                var list = Db.Connection.Query<sbtj>(sql.ToString());
                foreach (var item in list)
                {
                    var iscross = Tool.TimeTool.IsSameBC(item);
                    if (iscross)
                    {
                        Db.Connection.Execute("update SBTJ set lx = :lx,gxsj=sysdate where id = :id ", new {lx="1",id=item.id });
                    }
                    else
                    {
                        Db.Connection.Execute("update SBTJ set lx = :lx,gxsj=sysdate where id = :id ", new { lx = "0", id = item.id });
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

                var q = Db.Connection.Query<sbtj>(tsql.ToString());
                //停机数据
                foreach (var obj in q)
                {
                    //停机明细
                    var bctjsj_list = Tool.TimeTool.Calc_SBTJSD(obj);
                    foreach (var o in bctjsj_list)
                    {
                        Db.Connection.Execute(sql.ToString(), o);
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
