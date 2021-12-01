using LBJOEE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace LBJOEE.Services
{
    public class SBZTGXService: DBImp<sbztbhb>
    {
        public override dynamic Add(sbztbhb entity)
        {
            try
            {
                if (Tools.Tool.IsPing())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into sbztbhb(sbbh,sbzt,sbqy) ");
                    sql.Append(" values ");
                    sql.Append(" (:sbbh,:sbzt,:sbqy)");
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":sbbh", entity.sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    p.Add(":sbzt", entity.sbzt, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    p.Add(":sbqy", entity.sbqy, System.Data.DbType.String, System.Data.ParameterDirection.Input);

                    return Db.Connection.Execute(sql.ToString(), p);
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
                    return Db.Connection.Execute(sql_update.ToString(), new { sbbh = sbbh }) > 0 ? true : false;
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
                    return Db.Connection.Execute(sql_update.ToString(), new { sbbh = sbbh }) > 0 ? true : false;
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
