using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
using Dapper;
using DapperExtensions.Predicate;
using OEECalc.Model;
using log4net;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    public class SBXXService:OracleBaseFixture
    {
        private ILog log;
        private static readonly Lazy<SBXXService> lazy = new Lazy<SBXXService>(() => new SBXXService());
        private SBXXService()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public static SBXXService Instance { get { return lazy.Value; } }
        public IEnumerable<base_sbxx> Get_SBXX_List()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select sbbh, sbmc, sbxh, sbpp, sbzt, sfjx, sfhm, sfgz, sfql, sfqttj, jxkssj, hmkssj, gzkssj, qlkssj, qttjkssj, sbqy, ip, lrr, lrsj, tjms, port, cjgz, log, isupdate, issaveyssj, gxsj, scbz, djkssj, tjkssj, sfxm, sfts, xmkssj, tskssj, zcbh, sbgzsj, sbccbh, sbssbm, xt, sfby, bytjkssj, sflgtj, lgtjkssj, yxkssj ");
                sql.Append(" FROM   base_sbxx ");
                sql.Append(" where  scbz = 'N'");
                sql.Append(" order  by sbbh asc");
                
                    return db.Query<base_sbxx>(sql.ToString());
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sbxx>();
            }
        }
    }
}
