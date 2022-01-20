using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebOEE.Models;
using System.Text;
using Dapper;
namespace WebOEE.Services
{
    public class sbbzjpService: OracleBaseFixture
    {
        public bool Save_SBBZJP(List<sbbzjp> entitys)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" insert into sbbzjp(sbbh,bzjp) values (:sbbh,:bzjp) ");
                string hassql = "select count(sbbh) from sbbzjp where sbbh = :sbbh";
                string updatesql = "update sbbzjp set bzjp=:bzjp where sbbh=:sbbh";
                List<int> oklist = new List<int>();
                foreach (var item in entitys)
                {
                    int isok = 0;
                    var hasitem = Db.Connection.ExecuteScalar<int>(hassql, new { sbbh = item.sbbh });
                    if (hasitem == 0)
                    {
                        isok = Db.Connection.Execute(sql.ToString(), new { sbbh = item.sbbh, bzjp = item.bzjp });
                    }
                    else
                    {
                        isok = Db.Connection.Execute(updatesql, new { sbbh = item.sbbh, bzjp = item.bzjp });
                    }
                    oklist.Add(isok);
                }
                if (oklist.Where(t => t > 0).Count() == entitys.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<base_sbxx> SBXX_List()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select ta.sbbh, ta.sbqy,ta.sbxh, tb.bzjp ");
                sql.Append(" FROM(select sbbh,sbxh, sbqy FROM base_sbxx where scbz = 'N') ta, sbbzjp tb ");
                sql.Append(" where  ta.sbbh = tb.sbbh(+) ");
                sql.Append(" order  by ta.sbqy asc");
                return Db.Connection.Query<base_sbxx>(sql.ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}