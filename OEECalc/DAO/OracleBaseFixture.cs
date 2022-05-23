using DapperExtensions;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace OEECalc
{
    public class OracleBaseFixture : IDisposable
    {
        private string connstr = string.Empty;
        private OracleConnection connection;
        protected IDatabase Db;
        public OracleBaseFixture()
        {
            this.connstr = ConfigurationManager.ConnectionStrings["oee"] ?.ToString();
            InitDB();
        }

        public OracleConnection db { get { return connection; } }
        public string Constr
        {
            get
            {
                return connstr;
            }
        }
        public OracleBaseFixture(string connstr)
        {
            this.connstr = ConfigurationManager.ConnectionStrings[connstr] ?.ToString();
            InitDB();
        }

        public void Dispose()
        {
            connection.Dispose();
            Db.Dispose();
        }

        private void InitDB()
        {
            var config = new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new OracleDialect());
            var sqlGenerator = new SqlGeneratorImpl(config);
            connection = new OracleConnection(this.connstr);
            Db = new Database(connection, sqlGenerator);
        }
    }
}