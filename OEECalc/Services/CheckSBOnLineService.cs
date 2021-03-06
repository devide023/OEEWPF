using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEECalc.Tool;
using OEECalc.Model;
using Dapper;
using DapperExtensions;
using Newtonsoft.Json;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    public class CheckSBOnLineService : OracleBaseFixture
    {
        //private static CheckSBOnLineService instance = null;
        //private static readonly object padlock = new object();
        private static readonly Lazy<CheckSBOnLineService> lazy = new Lazy<CheckSBOnLineService>(() => new CheckSBOnLineService());
        private ILog log;
        private List<sbzx> _sbzx_list = new List<sbzx>();
        private CheckSBOnLineService()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public static CheckSBOnLineService Instance { get { return lazy.Value; } }
        //public static CheckSBOnLineService Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (instance == null)
        //                {
        //                    instance = new CheckSBOnLineService();
        //                }
        //            }
        //        }
        //        return instance;
        //    }
        //}

        public IEnumerable<base_sbxx> Get_SbList()
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select sbbh, ip, sbqy,sbzt FROM base_sbxx where scbz = 'N' order by sbbh asc");
                    return db.Query<base_sbxx>(sql.ToString());
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sbxx>();
            }
        }

        public void CheckOnLine()
        {
            try
            {
                
                    int ret = 0;
                    var list = Get_SbList();
                    StringBuilder sql = new StringBuilder();
                    sql.Append(" begin \n");
                    sql.Append(" update base_sbxx set tjkssj = sysdate where sbbh=:sbbh and sbzt='运行' and tjkssj is null;\n");
                    sql.Append(" update base_sbxx set djkssj = NULL where sbbh=:sbbh and sbzt='运行' and djkssj is not null;\n");
                    sql.Append(" update base_sbxx set yxkssj = NULL where sbbh=:sbbh and sbzt='运行' and yxkssj is not null;\n");
                    sql.Append(" commit;\n");
                    sql.Append(" end;\n");
                    StringBuilder sql1 = new StringBuilder();
                    sql1.Append("update base_sbxx set tjkssj = NULL where sbbh=:sbbh and sbzt='运行' and tjkssj is not null ");
                    foreach (var item in list)
                    {
                        var isok = Tool.NetCheck.IsPing(item.ip);
                        if (!isok)
                        {
                            ret = db.Execute(sql.ToString(), new { sbbh = item.sbbh });
                        }
                        else
                        {
                            ret = db.Execute(sql1.ToString(), new { sbbh = item.sbbh });
                        }
                    }
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
    }
}
