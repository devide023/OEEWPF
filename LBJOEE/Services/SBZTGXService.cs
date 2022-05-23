using LBJOEE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace LBJOEE.Services
{
    public class SBZTGXService : DBImp<sbztbhb>
    {
        public int AddByDate(sbztbhb entity)
        {
            try
            {                   
                    int sjjg = 3;
                    StringBuilder sql = new StringBuilder();
                    sql.Append(" declare \n");
                    sql.Append(" v_sbzt varchar(50); \n");
                    sql.Append(" v_sbqy varchar(50); \n");
                    sql.Append(" v_sfcj number; \n");
                    sql.Append(" v_kssj date; \n");//开始时间
                    sql.Append(" v_jssj date; \n");//结束时间
                    sql.Append(" v_tempsj date; \n");
                    sql.Append(" v_sjjg number; \n");//时间间隔
                    sql.Append(" begin \n");
                    sql.Append(" select :sj into v_tempsj from dual;\n");
                    sql.Append($" select {sjjg} into v_sjjg from dual;\n");
                    sql.Append(" select to_date( to_char(v_tempsj,'yyyy-mm-dd hh24')||':'||to_char(to_number(to_char(v_tempsj,'mi')) - mod(to_number(to_char(v_tempsj,'mi')), v_sjjg))||':00','yyyy-mm-dd hh24:mi:ss') into v_kssj FROM dual;\n");
                    sql.Append(" select v_kssj + numtodsinterval(v_sjjg,'minute') into v_jssj from dual;\n");
                    sql.Append(" select sbzt,sbqy into v_sbzt,v_sbqy from base_sbxx where sbbh = :sbbh;\n");
                    sql.Append(" insert into sbztbhb(sj,sbbh,sbzt,sbqy) ");
                    sql.Append(" select :sj,:sbbh,v_sbzt,v_sbqy from dual; \n");
                    sql.Append(" select count(sbbh) into v_sfcj from sbztbhb where sbbh=:sbbh and sj between v_kssj and v_jssj;\n");
                    sql.Append(" insert into sbcjtj(kssj,sbbh,sfcj) values(v_kssj,:sbbh,v_sfcj);\n");
                    sql.Append(" commit;\n");
                    sql.Append(" exception \n");
                    sql.Append(" when others then \n");
                    sql.Append(" rollback; \n");
                    sql.Append(" return;  \n");
                    sql.Append(" end; \n");
                    return db.Execute(sql.ToString(), entity);
                
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("LBJOEE.Services.SBZTGXService").Error(e.Message);
                return 0;
            }
        }
        public override dynamic Add(sbztbhb entity)
        {
            try
            {
                  
                    if (Tools.Tool.IsPing())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.Append(" begin \n");
                        sql.Append(" insert into sbztbhb(sbbh,sbzt,sbqy) ");
                        sql.Append(" values ");
                        sql.Append(" (:sbbh,:sbzt,:sbqy);\n");
                        sql.Append(" update base_sbxx set djkssj=NULL where sbbh= :sbbh and djkssj is not null;\n");
                        sql.Append(" update base_sbxx set tjkssj=NULL where sbbh= :sbbh and tjkssj is not null;\n");
                        sql.Append(" commit;\n");
                        sql.Append(" end;\n");
                        DynamicParameters p = new DynamicParameters();
                        p.Add(":sbbh", entity.sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                        p.Add(":sbzt", entity.sbzt, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                        p.Add(":sbqy", entity.sbqy, System.Data.DbType.String, System.Data.ParameterDirection.Input);

                        return db.Execute(sql.ToString(), p);
                    }
                    else
                    {
                        return 0;
                    }
                
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 设置待机时间
        /// </summary>
        /// <param name="sbbh"></param>
        /// <returns></returns>
        public bool Set_Djsj(string sbbh)
        {
            try
            {
                
                    StringBuilder sql_update = new StringBuilder();
                    sql_update.Append("update base_sbxx set djkssj = sysdate where sbbh = :sbbh ");
                    StringBuilder sql_tj = new StringBuilder();
                    sql_tj.Append("update base_sbxx set tjkssj = sysdate where sbbh = :sbbh ");
                    if (Tools.Tool.IsPing())
                    {
                        return db.Execute(sql_update.ToString(), new { sbbh = sbbh }) > 0 ? true : false;
                    }
                    else
                    {
                        return false;
                    }
                
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 取消待机时间
        /// </summary>
        /// <param name="sbbh"></param>
        /// <returns></returns>
        public bool UnSet_Djsj(string sbbh)
        {
            try
            {
                
                    StringBuilder sql_update = new StringBuilder();
                    sql_update.Append("update base_sbxx set djkssj = null where sbbh = :sbbh ");
                    StringBuilder sql_tj = new StringBuilder();
                    sql_tj.Append("update base_sbxx set tjkssj = sysdate where sbbh = :sbbh ");
                    if (Tools.Tool.IsPing())
                    {
                        return db.Execute(sql_update.ToString(), new { sbbh = sbbh }) > 0 ? true : false;
                    }
                    else
                    {
                        return false;
                    }

                
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
