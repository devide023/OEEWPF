using DapperExtensions;
using DapperExtensions.Predicate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Oracle;
using log4net;
namespace LBJOEE.Services
{
    /// <summary>
    /// 设备停机时间服务
    /// </summary>
    public class SBTJService:DBImp<sbtj>
    {
        private ILog log;
        public SBTJService()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        /// <summary>
        /// 计算设备停机时间
        /// </summary>
        /// <returns></returns>
        public bool CalcTjsj()
        {
            SBXXService sbxxservice = SBXXService.Instance;
            base_sbxx sbxx = sbxxservice.Find_Sbxx_ByIp();
            sbtj entity = new sbtj();
            entity.sbbh = sbxx.sbbh;
            DateTime kssj = DateTime.Now;
            if (sbxx.sfgz=="Y")
            {
                kssj = sbxx.gzkssj;
                entity.tjlx = "故障";
            }
            if (sbxx.sfjx == "Y")
            {
                kssj = sbxx.jxkssj;
                entity.tjlx = "检修";
            }
            if (sbxx.sfql == "Y")
            {
                kssj = sbxx.qlkssj;
                entity.tjlx = "缺料";
            }
            if (sbxx.sfhm == "Y")
            {
                kssj = sbxx.hmkssj;
                entity.tjlx = "换模";
            }
            if (sbxx.sfqttj == "Y")
            {
                kssj = sbxx.qttjkssj;
                entity.tjlx = "其他";
            }
            var jssj = DateTime.Now;
            entity.tjkssj = kssj;
            entity.tjjssj = jssj;
            entity.tjms = sbxx.tjms;
            entity.tjsj = CalSJC(kssj, jssj);
            var id = Add(entity);
            return (long)id > 0 ? true : false;;
        }

        public int CalSJC(DateTime d1,DateTime d2)
        {
            var ts = d2 - d1;
            return (int)ts.TotalMinutes;
        }

        public IEnumerable<sbtj> QueryTjList(DateTime ksrq,DateTime jsrq,string sbbh)
        {
            try
            {
                List<sbtj> list = new List<sbtj>();
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters p = new OracleDynamicParameters();
                p.Add(":sbbh", sbbh, OracleMappingType.Varchar2, System.Data.ParameterDirection.Input);
                p.Add(":ksrq", ksrq, OracleMappingType.Date, System.Data.ParameterDirection.Input);
                p.Add(":jsrq", jsrq, OracleMappingType.Date, System.Data.ParameterDirection.Input);
                sql.Append(" select sbbh, tjlx, tjsj, tjkssj, tjjssj, tjms ");
                sql.Append(" from SBTJ ");
                sql.Append(" where trunc(tjjssj) between trunc(:ksrq) and trunc(:jsrq) ");
                sql.Append(" and sbbh = :sbbh");
                return Db.Connection.Query<sbtj>(sql.ToString(), p);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                ErrorAction?.Invoke(e.Message);
                return new List<sbtj>();
            }
        }
    }
}
