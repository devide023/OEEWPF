﻿using DapperExtensions;
using DapperExtensions.Predicate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LBJOEE.Tools;
using Dapper;
using Dapper.Oracle;
using LBJOEE.Models;

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
        public bool IsAppUpdate()
        {
            try
            {
                string ip = Tool.GetIpAddress();
                string sql = "select isupdate FROM base_sbxx where ip = :ip ";
                string sfgx = Db.Connection.ExecuteScalar<string>(sql, new { ip = ip });
                return sfgx == "Y";
            }
            catch (Exception)
            {
                return false;
            }
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
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":cjgz", entity.cjgz, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfgz == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfgz='Y',gzkssj=sysdate,tjms=:tjms,cjgz=:cjgz,gxsj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfgz='N',tjms='',cjgz=:cjgz,gxsj=sysdate where sbbh=:sbbh ");
                }
                var ret = Db.Connection.Execute(sql.ToString(), q);
                return ret > 0 ? true : false;
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke("SBXXService.SetGZtj" + e.Message);
                return false;
            }
        }
        public bool SetJXtj(base_sbxx entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfjx == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfjx='Y',jxkssj=sysdate,tjms=:tjms,gxsj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfjx='N',tjms='',gxsj=sysdate where sbbh=:sbbh ");
                }
                var ret = Db.Connection.Execute(sql.ToString(), q);
                return ret > 0 ? true : false;
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke("SBXXService.SetJXtj" + e.Message);
                return false;
            }
        }
        public bool SetQLtj(base_sbxx entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfql == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfql='Y',qlkssj=sysdate,tjms=:tjms,gxsj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfql='N',tjms='',gxsj=sysdate where sbbh=:sbbh ");
                }
                var ret = Db.Connection.Execute(sql.ToString(), q);
                return ret > 0 ? true : false;
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke("SBXXService.SetQLtj" + e.Message);
                return false;
            }
        }
        public bool SetHMtj(base_sbxx entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfhm == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfhm='Y',hmkssj=sysdate,tjms=:tjms,gxsj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfhm='N',tjms='',gxsj=sysdate where sbbh=:sbbh ");
                }
                var ret = Db.Connection.Execute(sql.ToString(), q);
                return ret > 0 ? true : false;
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke("SBXXService.SetHMtj" + e.Message);
                return false;
            }
        }
        public bool SetQTtj(base_sbxx entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":xtsj", DateTime.Now, OracleMappingType.Date, System.Data.ParameterDirection.Input);
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfqttj == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfqttj='Y',qttjkssj=sysdate,tjms=:tjms,gxsj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfqttj='N',tjms='',gxsj=sysdate where sbbh=:sbbh ");
                }
                var ret = Db.Connection.Execute(sql.ToString(), q);
                return ret > 0 ? true : false;
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke("SBXXService.SetQTtj" + e.Message);
                return false;
            }
        }

        public IEnumerable<dygx> GetDYGX(string sbbh)
        {
            try
            {
                var sfcz = Db.Connection.ExecuteScalar<int>("select count(id) FROM sbcsconf where sbbh = :sbbh", new { sbbh = sbbh });
                if (sfcz == 0)
                {
                    return Db.Connection.Query<dygx>("select id, txt, colname, status,seq,width from DYGX where status = 1 ");
                }
                else
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select ta.id, ta.txt, ta.colname, ta.status, ta.seq, ta.width ");
                    sql.Append(" FROM   dygx ta, sbcsconf tb ");
                    sql.Append(" where  ta.id = tb.csid ");
                    sql.Append(" and    ta.status = 1 ");
                    sql.Append(" and    tb.sbbh = :sbbh");
                    return Db.Connection.Query<dygx>(sql.ToString(), new { sbbh = sbbh });
                }
            }
            catch (Exception)
            {
                return new List<dygx>();
            }
        }

        public DateTime GetServerTime()
        {
            try
            {
                DateTime result;
                string sql = "select sysdate from dual";
                string dt = Db.Connection.ExecuteScalar<string>(sql);
                if (DateTime.TryParse(dt, out result))
                {
                    return result;
                }
                else
                {
                    return DateTime.Now;
                }
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke("SBXXService.GetServerTime" + e.Message);
                return DateTime.Now;
            }
        }
    }
}
