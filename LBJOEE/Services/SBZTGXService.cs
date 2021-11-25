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
    }
}
