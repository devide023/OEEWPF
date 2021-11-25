using Dapper;
using System;
using System.Collections.Generic;
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
                return Db.Connection.ExecuteScalar<DateTime>(sql);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
    }
}
