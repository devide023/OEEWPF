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
using System.Configuration;
using System.IO;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    /// <summary>
    /// 检查是否有数据上传，并设置待机时间
    /// </summary>
    public class Check_DataUploadService:OracleBaseFixture
    {
        //private static Check_DataUploadService instance = null;
        //private static readonly object padlock = new object();
        private static readonly Lazy<Check_DataUploadService> lazy = new Lazy<Check_DataUploadService>(() => new Check_DataUploadService());
        private List<sys_sjsc> _global_cnf = null;
        private ILog log;
        private Check_DataUploadService()
        {
            log = LogManager.GetLogger(this.GetType());
            _global_cnf = new List<sys_sjsc>();
        }
        public static Check_DataUploadService Instance { get { return lazy.Value; } }
        //public static Check_DataUploadService Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                if (instance == null)
        //                {
        //                    instance = new Check_DataUploadService();
        //                }
        //            }
        //        }
        //        return instance;
        //    }
        //}

        private IEnumerable<base_sbxx> Get_SBXX_List()
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select sbbh, ip, sbqy,sbzt,sfjx,sfhm,sfgz,sfql,sfqttj,sfxm,sfts,sfby,sflgtj FROM base_sbxx where scbz = 'N' order by sbbh asc");
                    return db.Query<base_sbxx>(sql.ToString());
                
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
        public IEnumerable<sbztbhb> Get_ZTBH_List(string sbbh,int djsj=5)
        {
            try
            {
                
                    StringBuilder sql = new StringBuilder();
                    sql.Append("select id, sj, sbbh, sbzt, sbqy ");
                    sql.Append(" from sbztbhb ");
                    sql.Append(" where sbbh = :sbbh ");
                    sql.Append(" and    sj between sysdate - (1 / 24 / 60) * " + djsj + " and sysdate order by sj desc");
                    var list = db.Query<sbztbhb>(sql.ToString(), new { sbbh = sbbh });
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
                    return db.Execute(sql.ToString(), p) > 0 ? true : false;
                
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
                    sql.Append(" begin \n");
                    sql.Append(" update BASE_SBXX set djkssj = sysdate where sbbh = :sbbh and sbzt = '运行' and djkssj is null; \n");
                    sql.Append(" commit;\n");
                    sql.Append(" end; \n");
                    DynamicParameters p = new DynamicParameters();
                    p.Add(":sbbh", sbbh, System.Data.DbType.String, System.Data.ParameterDirection.Input);
                    return db.Execute(sql.ToString(), p) > 0 ? true : false;
                
                
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
                    sql.Append(" begin \n");
                    sql.Append(" update BASE_SBXX set djkssj = NULL where sbbh = :sbbh and sbzt = '运行' and djkssj is not null;\n");
                    sql.Append(" commit;\n");
                    sql.Append(" end; \n");
                    return db.Execute(sql.ToString(), new { sbbh = sbbh }) > 0 ? true : false;
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 设备待机时间配置
        /// </summary>
        /// <returns></returns>
        public List<sys_scsjconf> DJSJConf()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string fullpath = path + "\\" + "djsjconf.json";
                using (StreamReader sr = new StreamReader(fullpath))
                {
                    string json = sr.ReadToEnd();
                    List<sys_scsjconf> list = JsonConvert.DeserializeObject<List<sys_scsjconf>>(json);
                    return list;
                }
            }
            catch (Exception)
            {
                return new List<sys_scsjconf>();
            }
        }

        public void NewCheck()
        {
            try
            {
                var sjjgconf = DJSJConf();
                var sbxxlist = Get_SBXX_List();
                foreach (var item in sbxxlist)
                {
                    Int32 sbsjjg = 5;
                    var sjjg_query = sjjgconf.Where(t => t.sbbh == item.sbbh);
                    if (sjjg_query.Count() > 0)
                    {
                        sbsjjg = sjjg_query.First().sjjg;
                    }
                    var datalist = Get_ZTBH_List(item.sbbh, sbsjjg);
                    //5分钟内没有数据上传
                    if (datalist.Count() == 0)
                    {
                        Set_SbDj_SJ(item.sbbh);
                    }
                    else//有数据上传
                    {
                        UnSet_SbDj_SJ(item.sbbh);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        public void Check()
        {            
            try
            {
                var sjjgconf = DJSJConf();
                var sbxxlist = Get_SBXX_List();
                foreach (var item in sbxxlist.Where(t=>t.sbzt =="运行"))
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
                        e.js = 0m;
                        e.sbzt = string.Empty;
                        _global_cnf.Add(e);
                        pos = _global_cnf.FindIndex(t => t.sbbh == item.sbbh);
                    }
                    Int32 sbsjjg = 5;
                    var sjjg_query = sjjgconf.Where(t => t.sbbh == item.sbbh);
                    if (sjjg_query.Count() > 0)
                    {
                        sbsjjg = sjjg_query.First().sjjg;
                    }
                    var datalist = Get_ZTBH_List(item.sbbh, sbsjjg);
                    var isok = NetCheck.IsPing(item.ip);
                    if (isok)
                    {
                        //5分钟内没有数据上传
                        if (datalist.Count() == 0)
                        {
                            //没有手动停机情况下更新设备待机时间
                            if (_global_cnf[pos].js == 0 && item.sfgz == "N" && item.sfhm == "N" && item.sfjx == "N" && item.sfql == "N" && item.sfqttj == "N" && item.sfxm == "N" && item.sfts == "N" && item.sfby=="N" && item.sflgtj=="N" )
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
                            UnSet_SbDj_SJ(item.sbbh);
                            _global_cnf[pos].sbzt = firstzx.sbzt;
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
