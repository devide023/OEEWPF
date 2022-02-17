using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OEECalc.Tool;
using OEECalc.Model;
using Dapper;
using DapperExtensions;
using Newtonsoft.Json;
namespace OEECalc.Services
{
    public class CheckSBOnLineService:OracleBaseFixture
    {
        private static CheckSBOnLineService instance = null;
        private static readonly object padlock = new object();
        private ILog log;
        private List<sbzx> _sbzx_list = new List<sbzx>();
        private CheckSBOnLineService()
        {
            log = LogManager.GetLogger(this.GetType());
            var sblist = Get_SbList();
            foreach (var item in sblist)
            {
                _sbzx_list.Add(new sbzx()
                {
                    ip = item.ip,
                    sbbh=item.sbbh,
                    sbzt = item.sbzt,
                    isonline = true,
                });
            }
        }

        public static CheckSBOnLineService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new CheckSBOnLineService();
                        }
                    }
                }
                return instance;
            }
        }

        public IEnumerable<base_sbxx> Get_SbList()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select sbbh, ip, sbqy,sbzt FROM base_sbxx where scbz = 'N' order by sbbh asc");
                return Db.Connection.Query<base_sbxx>(sql.ToString());
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sbxx>();
            }
        }

        public void CheckOnLine()
        {
            try
            {
                var list = Get_SbList();
                StringBuilder sql = new StringBuilder();
                sql.Append("update base_sbxx set tjkssj = sysdate where sbbh=:sbbh ");
                StringBuilder sql1 = new StringBuilder();
                sql1.Append("update base_sbxx set tjkssj = NULL where sbbh=:sbbh ");

                foreach (var item in list)
                {
                    var q = _sbzx_list.Where(t => t.sbbh == item.sbbh);
                    var isok = Tool.NetCheck.IsPing(item.ip);
                    //有新增设备加入全局变量
                    if (q.Count() == 0)
                    {
                        _sbzx_list.Add(new sbzx()
                        {
                            ip = item.ip,
                            sbbh = item.sbbh,
                            sbzt = item.sbzt,
                            isonline = isok,
                        });
                    }
                    else//原有设备
                    {
                        var fitem = q.FirstOrDefault();
                        if (fitem != null)
                        {
                            if(fitem.isonline != isok)
                            {
                                //更新数据表,当设备不能ping通时
                                if (!isok)
                                {
                                    Db.Connection.Execute(sql.ToString(), new { sbbh = item.sbbh });
                                }
                                else
                                {
                                    Db.Connection.Execute(sql1.ToString(), new { sbbh = item.sbbh });
                                }
                                fitem.isonline = isok;
                            }
                        }
                    }
                }
                //log.Info(JsonConvert.SerializeObject(_sbzx_list));

            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
    }
}
