using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Dapper;
using DapperExtensions;
using OEECalc.Tool;
using OEECalc.Model;
using Newtonsoft.Json;
namespace OEECalc.Services
{
    public class Check_DataUploadService:OracleBaseFixture
    {
        private static Check_DataUploadService instance = null;
        private static readonly object padlock = new object();
        private List<sys_sjsc> _global_cnf = null;
        private ILog log;
        private Check_DataUploadService()
        {
            log = LogManager.GetLogger(this.GetType());
            _global_cnf = new List<sys_sjsc>();
        }
        public static Check_DataUploadService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new Check_DataUploadService();
                        }
                    }
                }
                return instance;
            }
        }

        private IEnumerable<base_sbxx> Get_SBXX_List()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select sbbh, ip, sbqy,sbzt,sfjx,sfhm,sfgz,sfql,sfqttj,sfxm,sfts FROM base_sbxx where scbz = 'N' order by sbbh asc");
                return Db.Connection.Query<base_sbxx>(sql.ToString());
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sbxx>();
            }
        }

        /// <summary>
        /// 获取设备最近5分钟内的状态变化
        /// </summary>
        /// <returns></returns>
        public IEnumerable<sbztbhb> Get_ZTBH_List(string sbbh)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select id, sj, sbbh, sbzt, sbqy ");
                sql.Append(" from sbztbhb ");
                sql.Append(" where sbbh = :sbbh ");
                sql.Append(" and    sj between sysdate - (1 / 24 / 60) * 5 and sysdate order by sj desc");
                var list = Db.Connection.Query<sbztbhb>(sql.ToString(), new { sbbh = sbbh });
                return list;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<sbztbhb>();
            }
        }

        /// <summary>
        /// 设置设备待机时间
        /// </summary>
        /// <returns></returns>
        public bool Set_SbDj_SJ(string sbbh, DateTime djsj)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("update BASE_SBXX set djkssj = :djsj where sbbh = :sbbh");
                DynamicParameters p = new DynamicParameters();
                p.Add(":djsj", djsj, System.Data.DbType.Date, System.Data.ParameterDirection.Input);
                p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                return Db.Connection.Execute(sql.ToString(), p) > 0 ? true : false;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 设置待机时间
        /// </summary>
        /// <param name="sbbh"></param>
        /// <returns></returns>
        public bool Set_SbDj_SJ(string sbbh)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("update BASE_SBXX set djkssj = sysdate where sbbh = :sbbh");
                DynamicParameters p = new DynamicParameters();
                p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                return Db.Connection.Execute(sql.ToString(), p) > 0 ? true : false;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 取消待机时间
        /// </summary>
        /// <param name="sbbh"></param>
        /// <returns></returns>
        public bool UnSet_SbDj_SJ(string sbbh)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("update BASE_SBXX set djkssj = NULL where sbbh = :sbbh");
                return Db.Connection.Execute(sql.ToString(), new { sbbh = sbbh }) > 0 ? true : false;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }

        public void Check()
        {
            //log.Info(JsonConvert.SerializeObject(_global_cnf) + "\r\n");
            try
            {
                var sbxxlist = Get_SBXX_List();
                foreach (var item in sbxxlist)
                {
                    int pos = -1;
                    var q = _global_cnf.Where(t => t.sbbh == item.sbbh);
                    if (q.Count() > 0)
                    {
                        pos = _global_cnf.FindIndex(t => t.sbbh == item.sbbh);
                    }
                    else
                    {
                        var e = new sys_sjsc();
                        e.sbbh = item.sbbh;
                        e.js = 0;
                        e.sbzt = string.Empty;
                        _global_cnf.Add(e);
                        pos = _global_cnf.FindIndex(t => t.sbbh == item.sbbh);
                    }
                    var datalist = Get_ZTBH_List(item.sbbh);
                    var isok = NetCheck.IsPing(item.ip);
                    if (isok)
                    {
                        if (datalist.Count() == 0)
                        {
                            //没有手动停机情况下更新设备待机时间
                            if (item.sfgz == "N" && item.sfhm == "N" && item.sfjx == "N" && item.sfql == "N" && item.sfqttj == "N" && item.sfxm == "N" && item.sfts == "N" && _global_cnf[pos].js == 0)
                            {
                                Set_SbDj_SJ(item.sbbh);
                                _global_cnf[pos].js = _global_cnf[pos].js + 0.1m;
                                _global_cnf[pos].sbzt = "待机";
                            }
                        }
                        else//有数据上传
                        {
                            _global_cnf[pos].js = 0m;
                            var firstzx = datalist.OrderByDescending(t => t.sj).FirstOrDefault();//最新一条数据
                            if (firstzx.sbzt == "运行")
                            {
                                UnSet_SbDj_SJ(item.sbbh);
                                _global_cnf[pos].sbzt = firstzx.sbzt;
                            }
                            else
                            {
                                if (_global_cnf[pos].sbzt != firstzx.sbzt && firstzx.sbzt=="待机")
                                {
                                    //没有手动停机情况下更新设备待机时间
                                    if (item.sfgz == "N" && item.sfhm == "N" && item.sfjx == "N" && item.sfql == "N" && item.sfqttj == "N" && item.sfxm == "N" && item.sfts == "N")
                                    {
                                        Set_SbDj_SJ(item.sbbh);
                                    }
                                    _global_cnf[pos].sbzt = firstzx.sbzt;
                                }
                            }
                        }
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
