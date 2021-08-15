using DapperExtensions;
using DapperExtensions.Predicate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LBJOEE.Tools;
using Dapper;
using Dapper.Oracle;
namespace LBJOEE.Services
{
    /// <summary>
    /// 设备信息服务类
    /// </summary>
    public class SBXXService : DBImp<base_sbxx>
    {
        public base_sbxx Find_Sbxx_ByIp()
        {
            string ip = Tool.GetIpAddress();
            return Find_Sbxx_ByIp(ip);
        }
        public base_sbxx Find_Sbxx_ByIp(string ip)
        {
            try
            {
                var predicate = Predicates.Field<base_sbxx>(f => f.ip, Operator.Eq, ip);
                base_sbxx sbxx = Db.GetList<base_sbxx>(predicate).FirstOrDefault();
                return sbxx;
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke(e.Message);
                return null;
            }
        }
        public base_sbxx Find_Sbxx_ByBh(string sbbh)
        {
            var predicate = Predicates.Field<base_sbxx>(f => f.sbbh, Operator.Eq, sbbh);
            base_sbxx sbxx = Db.GetList<base_sbxx>(predicate).FirstOrDefault();
            return sbxx;
        }
        /// <summary>
        /// 设置故障停机描述及时间
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool SetGZtj(base_sbxx entity)
        {
            StringBuilder sql = new StringBuilder();
            OracleDynamicParameters q = new OracleDynamicParameters();
            q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
            q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            q.Add(":tjms", entity.tjms??"", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            if (entity.sfgz == "Y")
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfgz='Y',gzkssj=:xtsj,tjms=:tjms where sbbh=:sbbh ");
            }
            else
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfgz='N',tjms='' where sbbh=:sbbh ");
            }
            var ret = Db.Connection.Execute(sql.ToString(), q);
            return ret > 0 ? true : false;
        }
        public bool SetJXtj(base_sbxx entity)
        {
            StringBuilder sql = new StringBuilder();
            OracleDynamicParameters q = new OracleDynamicParameters();
            q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
            q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            q.Add(":tjms", entity.tjms??"", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            if (entity.sfjx == "Y")
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfjx='Y',jxkssj=:xtsj,tjms=:tjms where sbbh=:sbbh ");
            }
            else
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfjx='N',tjms='' where sbbh=:sbbh ");
            }
            var ret = Db.Connection.Execute(sql.ToString(), q);
            return ret > 0 ? true : false;
        }
        public bool SetQLtj(base_sbxx entity)
        {
            StringBuilder sql = new StringBuilder();
            OracleDynamicParameters q = new OracleDynamicParameters();
            q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
            q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2,System.Data.ParameterDirection.Input);
            q.Add(":tjms", entity.tjms??"", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            if (entity.sfql == "Y")
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfql='Y',qlkssj=:xtsj,tjms=:tjms where sbbh=:sbbh ");
            }
            else
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfql='N',tjms='' where sbbh=:sbbh ");
            }
            var ret = Db.Connection.Execute(sql.ToString(), q);
            return ret > 0 ? true : false;
        }
        public bool SetHMtj(base_sbxx entity)
        {
            StringBuilder sql = new StringBuilder();
            OracleDynamicParameters q = new OracleDynamicParameters();
            q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
            q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            q.Add(":tjms", entity.tjms??"", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            if (entity.sfhm == "Y")
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfhm='Y',hmkssj=:xtsj,tjms=:tjms where sbbh=:sbbh ");
            }
            else
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfhm='N',tjms='' where sbbh=:sbbh ");
            }
            var ret = Db.Connection.Execute(sql.ToString(), q);
            return ret > 0 ? true : false;
        }
        public bool SetQTtj(base_sbxx entity)
        {
            StringBuilder sql = new StringBuilder();
            OracleDynamicParameters q = new OracleDynamicParameters();
            q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
            q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            q.Add(":tjms", entity.tjms??"", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
            if (entity.sfqttj == "Y")
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfqttj='Y',qttjkssj=:xtsj,tjms=:tjms where sbbh=:sbbh ");
            }
            else
            {
                sql.Append("update base_sbxx set sbzt=:sbzt,sfqttj='N',tjms='' where sbbh=:sbbh ");
            }
            var ret = Db.Connection.Execute(sql.ToString(), q);
            return ret > 0 ? true : false;
        }
    }
}
