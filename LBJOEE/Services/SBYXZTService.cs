using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
using DapperExtensions.Predicate;
using Dapper;
using LBJOEE.Models;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace LBJOEE.Services
{
    public class SBYXZTService : OracleBaseFixture
    {
        public SBYXZTService()
        {
        }
        /// <summary>
        /// 获取设备最近5分钟内的状态变化
        /// </summary>
        /// <returns></returns>
        public IEnumerable<sbztbhb> Get_ZTBH_List(string sbbh)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select id, sj, sbbh, sbzt, sbqy ");
                    sql.Append(" from sbztbhb ");
                    sql.Append(" where sbbh = :sbbh ");
                    sql.Append(" and    sj between sysdate - (1 / 24 / 60) * 5 and sysdate");
                    var list = db.Query<sbztbhb>(sql.ToString(), new { sbbh = sbbh });
                    return list;
                
            }
            catch (Exception)
            {
                return new List<sbztbhb>();
            }
        }
        /// <summary>
        /// 设置设备待机时间
        /// </summary>
        /// <returns></returns>
        public bool Set_SbDj_SJ(string sbbh, DateTime djsj)
        {
            try
            {
               
                    StringBuilder sql = new StringBuilder();
                    sql.Append("update BASE_SBXX set djkssj = :djsj where sbbh = :sbbh");
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":djsj", djsj, System.Data.DbType.Date, System.Data.ParameterDirection.Input);
                    p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    return db.Execute(sql.ToString(), p) > 0 ? true : false;
                
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Set_SbDj_SJ(string sbbh)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("update BASE_SBXX set djkssj = sysdate where sbbh = :sbbh");
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    return db.Execute(sql.ToString(), p) > 0 ? true : false;
                
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UnSet_SbDj_SJ(string sbbh)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("update BASE_SBXX set djkssj = NULL where sbbh = :sbbh");
                    return db.Execute(sql.ToString(), new { sbbh = sbbh }) > 0 ? true : false;
                
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
