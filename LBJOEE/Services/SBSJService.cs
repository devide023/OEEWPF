using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LBJOEE.Models;
using Dapper;
using System.Data;
using LBJOEE.Tools;

namespace LBJOEE.Services
{
    /// <summary>
    /// 设备数据服务类
    /// </summary>
    public class SBSJService:DBImp<sjcj>
    {
        private static SBSJService instance = null;
        private static readonly object padlock = new object();

        private SBSJService()
        {

        }

        public static SBSJService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new SBSJService();
                        }
                    }
                }
                return instance;
            }
        }

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
                    Db.Connection.Execute(sql.ToString(), p);
                }
                else
                {
                    DataBackUp.SaveOrginalDataToLocal(entity);
                }
            }
            catch (Exception)
            {
                //Environment.Exit(0);
            }
        }
    }
}
