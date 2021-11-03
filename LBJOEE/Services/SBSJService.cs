using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LBJOEE.Models;
using Dapper;
using System.Data;
namespace LBJOEE.Services
{
    /// <summary>
    /// 设备数据服务类
    /// </summary>
    public class SBSJService:DBImp<sjcj>
    {
        public void SaveOriginalData(originaldata entity)
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
            Db.Connection.Execute(sql.ToString(), p);
        }
    }
}
