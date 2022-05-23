using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OEECalc.Tool
{
    public class TimeUtil: OracleBaseFixture
    {
        public DateTime ServerTime()
        {
            try
            {
                
                    string sql = "select sysdate from dual";
                    return db.ExecuteScalar<DateTime>(sql);
                
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
    }
}
