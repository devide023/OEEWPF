using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Dapper;
using OEECalc.Model;

namespace OEECalc.Services
{
    /// <summary>
    /// 发送短信服务
    /// </summary>
    public class SendSMSServic:OracleBaseFixture
    {
        private DjTJService djtjservice = null;
        private ILog log;
        public SendSMSServic(string conn):base(conn)
        {
            log = LogManager.GetLogger(this.GetType());
            djtjservice = new DjTJService();
        } 
        /// <summary>
        /// 保存信息到短信中间表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private int Save_To_MidTable(sys_sms entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into SMS_INFORMATION(content,Phone) values(:txt,:tel)");
                int ret = Db.Connection.Execute(sql.ToString(), new { txt = entity.context, tel = entity.tel });
                return ret;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0;
            }
        }

        public void SendMSg()
        {
            try
            {
                var sendlist = djtjservice.Check();
                var djlist = sendlist.Where(t => t.cklx == "待机").ToList();
                var tjlist = sendlist.Where(t => t.cklx == "脱机").ToList();
                string djjt = string.Empty;
                string tjjt = string.Empty;
                //要发送的内容
                string send_context = string.Empty;
                djlist.ForEach(t =>
                {
                    djjt = djjt + t.sbqy + "、";
                });
                tjlist.ForEach(t =>
                {
                    tjjt = tjjt + t.sbqy + "、";
                });
                if (!string.IsNullOrEmpty(djjt) || !string.IsNullOrEmpty(tjjt))
                {
                    List<string> parmlist = new List<string>();
                    if (string.IsNullOrEmpty(djjt))
                    {
                        djjt = "无";
                    }
                    else
                    {
                        djjt = djjt.Remove(djjt.Length - 1);
                    }
                    if (string.IsNullOrEmpty(tjjt))
                    {
                        tjjt = "无";
                    }
                    else
                    {
                        tjjt = tjjt.Remove(tjjt.Length - 1);
                    }
                    parmlist.Add(djjt);
                    parmlist.Add(tjjt);
                    //获取发送短信对象列表
                    var fsdxlist = djtjservice.Get_SmsList().Where(t=>t.state == 1);
                    foreach (var item in fsdxlist)
                    {
                        send_context = string.Empty;
                        //短信模板
                        var dxmb = djtjservice.Get_SMS_Temp(item.sms_id);
                        //短信模板参数
                        var parlist = djtjservice.Get_Temp_Parm(item.sms_id).ToList();
                        for (int i = 0; i < parlist.Count(); i++)
                        {
                            dxmb.sms_temp = dxmb.sms_temp.Replace(parlist[i].sms_parm, parmlist[i]);
                        }
                        send_context = dxmb.sms_temp;
                        //判断模板里的参数是否被替换
                        if (send_context.IndexOf("{") == -1)
                        {
                            var ret = Save_To_MidTable(new sys_sms()
                            {
                                context = send_context,
                                tel = item.tel
                            });
                            //保存成功
                            if (ret > 0)
                            {
                                djtjservice.UpdateSendTime(item.id);
                                djtjservice.SaveToLog(new base_sms_log()
                                {
                                    tel = item.tel,
                                    content = send_context,
                                    sendtime = DateTime.Now
                                });
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
