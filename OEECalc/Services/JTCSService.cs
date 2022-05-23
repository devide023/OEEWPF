using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEECalc.Model;
using Dapper;
using log4net;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    /// <summary>
    /// 机台参数服务类
    /// </summary>
    public class JTCSService : OracleBaseFixture
    {
        private ILog log;
        private static readonly Lazy<JTCSService> lazy = new Lazy<JTCSService>(() => new JTCSService());
        private JTCSService()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public static JTCSService Instance { get { return lazy.Value; } }
        public IEnumerable<base_sbxx> sbxxlist()
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select sbbh, sbqy FROM base_sbxx where scbz = 'N' order by sbqy asc");
                    return db.Query<base_sbxx>(sql.ToString());
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sbxx>();
            }
        }
        /// <summary>
        /// 机台参数
        /// </summary>
        public void JTCS()
        {
            
                var list = sbxxlist();
                StringBuilder sql = new StringBuilder();
                sql.Append("select json FROM  receivedata where sbbh=:sbbh and rq = ( ");
                sql.Append("select max(rq) from receivedata where sbbh = :sbbh ");
                sql.Append(")");
                StringBuilder output = new StringBuilder();
                foreach (var item in list)
                {
                    var json = db.ExecuteScalar<string>(sql.ToString(), new { sbbh = item.sbbh });
                    var csmc = JsonConvert.DeserializeObject<List<itemdata>>(json).Select(t => t.itemName);
                    string names = string.Empty;
                    csmc.ToList().ForEach(t => names = names + t + ",");
                    output.Append($"{item.sbqy} \t{names} \r\n");
                }
                using (StreamWriter sw = new StreamWriter(@"D:\jtcs.txt"))
                {
                    sw.WriteLine(output.ToString());
                }
            
        }
    }
}
