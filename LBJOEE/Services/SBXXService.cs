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
using LBJOEE.Models;

namespace LBJOEE.Services
{
    /// <summary>
    /// 设备信息服务类
    /// </summary>
    public class SBXXService : DBImp<base_sbxx>
    {
        private static SBXXService instance = null;
        private static readonly object padlock = new object();
        private SBXXService()
        {

        }

        public static SBXXService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new SBXXService();
                        }
                    }
                }
                return instance;
            }
        }

        public base_sbxx Find_Sbxx_ByIp()
        {
            string ip = Tool.GetIpAddress();
            return Find_Sbxx_ByIp(ip);
        }
        public bool IsAppUpdate()
        {
            try
            {
                if (Tool.IsPing())
                {
                    string ip = Tool.GetIpAddress();
                    string sql = "select isupdate FROM base_sbxx where ip = :ip ";
                    string sfgx = Db.Connection.ExecuteScalar<string>(sql, new { ip = ip });
                    return sfgx == "Y";
                }
                else
                {
                    return false;
                }
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
                StringBuilder sql = new StringBuilder();
                sql.Append("select sbbh, sbmc, sbxh, sbpp, sbzt, sfjx, sfhm, sfgz, sfql, sfqttj, jxkssj, hmkssj, gzkssj, qlkssj, qttjkssj, sbqy, ip, tjms, port, cjgz, log, isupdate, issaveyssj,sfxm,sfts,xmkssj,tskssj FROM   base_sbxx where ip = :ip");
                DynamicParameters p = new DynamicParameters();
                p.Add(":ip", ip, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                var q = Db.Connection.Query<base_sbxx>(sql.ToString(),p);
                if (q.Count() > 0)
                {
                    return q.First();
                }
                else
                {
                    return new base_sbxx();
                }
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke(e.Message);
                return null;
            }
        }
        public base_sbxx Find_Sbxx_ByBh(string sbbh)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select sbbh, sbmc, sbxh, sbpp, sbzt, sfjx, sfhm, sfgz, sfql, sfqttj, jxkssj, hmkssj, gzkssj, qlkssj, qttjkssj, sbqy, ip, lrr, lrsj, tjms, port, cjgz, log, isupdate, issaveyssj, gxsj, scbz, djkssj, tjkssj FROM   base_sbxx where sbbh = :sbbh");
                var q = Db.Connection.Query<base_sbxx>(sql.ToString(), new { sbbh = sbbh });
                if (q.Count() > 0)
                {
                    return q.First();
                }
                else
                {
                    return new base_sbxx();
                }
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke(e.Message);
                return new base_sbxx();
            }
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
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":cjgz", entity.cjgz, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfgz == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfgz='Y',gzkssj=sysdate,tjms=:tjms,cjgz=:cjgz,gxsj=sysdate,tjkssj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfgz='N',tjms='',cjgz=:cjgz,gxsj=sysdate,tjkssj=NULL where sbbh=:sbbh ");
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
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfjx == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfjx='Y',jxkssj=sysdate,tjms=:tjms,gxsj=sysdate,tjkssj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfjx='N',tjms='',gxsj=sysdate,tjkssj=NULL where sbbh=:sbbh ");
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
        public bool SetBYTJ(base_sbxx entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfby == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfby='Y',bytjkssj=sysdate,tjms=:tjms,gxsj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfby='N',bytjkssj=NULL,tjms=NULL,gxsj=sysdate where sbbh=:sbbh ");
                }
                var ret = Db.Connection.Execute(sql.ToString(), q);
                return ret > 0 ? true : false;
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke("SBXXService.SetBYTJ" + e.Message);
                return false;
            }
        }
        public bool SetQLtj(base_sbxx entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfql == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfql='Y',qlkssj=sysdate,djkssj=sysdate,tjms=:tjms,gxsj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfql='N',tjms='',djkssj = NULL,gxsj=sysdate where sbbh=:sbbh ");
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
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfhm == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfhm='Y',hmkssj=sysdate,tjms=:tjms,gxsj=sysdate,tjkssj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfhm='N',tjms='',gxsj=sysdate,tjkssj=NULL where sbbh=:sbbh ");
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
        public bool SetXMtj(base_sbxx entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfxm == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfxm='Y',xmkssj=sysdate,tjms=:tjms,gxsj=sysdate,tjkssj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfxm='N',tjms='',gxsj=sysdate,tjkssj=NULL where sbbh=:sbbh ");
                }
                var ret = Db.Connection.Execute(sql.ToString(), q);
                return ret > 0 ? true : false;
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke("SBXXService.SetXMtj" + e.Message);
                return false;
            }
        }
        public bool SetTStj(base_sbxx entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfts == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfts='Y',tskssj=sysdate,tjms=:tjms,gxsj=sysdate,tjkssj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfts='N',tjms='',gxsj=sysdate,tjkssj=NULL where sbbh=:sbbh ");
                }
                var ret = Db.Connection.Execute(sql.ToString(), q);
                return ret > 0 ? true : false;
            }
            catch (Exception e)
            {
                ErrorAction?.Invoke("SBXXService.SetTStj" + e.Message);
                return false;
            }
        }
        public bool SetQTtj(base_sbxx entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                OracleDynamicParameters q = new OracleDynamicParameters();
                q.Add(":sbbh", entity.sbbh, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":tjms", entity.tjms ?? "", OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                q.Add(":sbzt", entity.sbzt, OracleMappingType.NVarchar2, System.Data.ParameterDirection.Input);
                if (entity.sfqttj == "Y")
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfqttj='Y',qttjkssj=sysdate,djkssj = sysdate,tjms=:tjms,gxsj=sysdate where sbbh=:sbbh ");
                }
                else
                {
                    sql.Append("update base_sbxx set sbzt=:sbzt,sfqttj='N',tjms='',djkssj =NULL,gxsj=sysdate where sbbh=:sbbh ");
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
                    return Db.Connection.Query<dygx>("select id, txt, colname, status,seq,width,coltype from DYGX where status = 1 ");
                }
                else
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select ta.id, ta.txt, ta.colname, ta.status, ta.seq, ta.width,ta.coltype ");
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
