using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Dapper;
using OEECalc.Model;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OEECalc.Services
{
    /// <summary>
    /// 发送短信服务
    /// </summary>
    public class SendSMSServic:OracleBaseFixture
    {
        private static readonly Lazy<SendSMSServic> lazy = new Lazy<SendSMSServic>(() => new SendSMSServic("sms"));
        private DjTJService djtjservice = null;
        private ILog log;
        public SendSMSServic(string conn):base(conn)
        {
            log = LogManager.GetLogger(this.GetType());
            djtjservice = DjTJService.Instance;
        }
        public static SendSMSServic Instance { get { return lazy.Value; } }
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
                    int ret = db.Execute(sql.ToString(), new { txt = entity.context, tel = entity.tel });
                    return ret;
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0;
            }
        }
        /// <summary>
        /// 按人员所管辖的机台分组发送信息
        /// </summary>
        public void SendMsgByPerson()
        {
            try
            {
                var sendlist = djtjservice.Check_TJDJ_By_Person();
                //过滤手机号
                var q = sendlist.Where(t=>t.fl==0).Select(t => new { tel = t.tel, sms_id = t.sms_id }).Distinct();
                foreach (var item in q)
                {
                    var djtjlist = sendlist.Where(t => t.tel == item.tel);
                    var djlist = djtjlist.Where(t => t.djkssj != null && t.djsc >= 20m).ToList();
                    var tjlist = djtjlist.Where(t => t.tjkssj != null && t.tjsc >= 5m).ToList();
                    
                    string djjt = string.Empty;
                    string tjjt = string.Empty;
                    
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
                        //短信模板
                        var dxmb = djtjservice.Get_SMS_Temp(item.sms_id);
                        //短信模板参数
                        var parlist = djtjservice.Get_Temp_Parm(item.sms_id).OrderBy(t => t.id).ToList();
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
                        parmlist.Add(DateTime.Now.ToString("MM-dd HH:mm:ss"));
                        for (int i = 0; i < parlist.Count(); i++)
                        {
                            dxmb.sms_temp = dxmb.sms_temp.Replace(parlist[i].sms_parm, parmlist[i]);
                        }
                        SendSMSByTelPhone(dxmb.sms_temp, item.tel);
                    }
                    
                }
                //领导模板
                var q1 = sendlist.Where(t => t.fl == 1).Select(t => new { tel = t.tel, sms_id = t.sms_id }).Distinct();
                foreach (var item in q1)
                {
                    var djtjlist = sendlist.Where(t => t.tel == item.tel);
                    //待机超2小时
                    var dj2hlist = djtjlist.Where(t => t.djkssj != null && t.djsc >= 120m).ToList();
                    //脱机超2小时
                    var tj2hlist = djtjlist.Where(t => t.tjkssj != null && t.tjsc >= 120m).ToList();
                    string djjt2h = string.Empty;
                    string tjjt2h = string.Empty;
                    //超过2小时待机机台
                    dj2hlist.ForEach(t =>
                    {
                        djjt2h = djjt2h + t.sbqy + "、";
                    });
                    //超过2小时脱机机台
                    tj2hlist.ForEach(t =>
                    {
                        tjjt2h = tjjt2h + t.sbqy + "、";
                    });
                    //超过2小时待机，停机机台
                    if (!string.IsNullOrEmpty(djjt2h) || !string.IsNullOrEmpty(tjjt2h))
                    {
                        //短信模板
                        var dxmb = djtjservice.Get_SMS_Temp(item.sms_id);
                        //短信模板参数
                        var parlist = djtjservice.Get_Temp_Parm(item.sms_id).OrderBy(t => t.id).ToList();
                        List<string> parmlist = new List<string>();
                        if (!string.IsNullOrEmpty(djjt2h))
                        {
                            djjt2h = djjt2h.Remove(djjt2h.Length - 1);
                        }
                        else
                        {
                            djjt2h = "无";
                        }
                        if (!string.IsNullOrEmpty(tjjt2h))
                        {
                            tjjt2h = tjjt2h.Remove(tjjt2h.Length - 1);
                        }
                        else
                        {
                            tjjt2h = "无";
                        }
                        parmlist.Add(djjt2h);
                        parmlist.Add(tjjt2h);
                        parmlist.Add(DateTime.Now.ToString("MM-dd HH:mm:ss"));
                        for (int i = 0; i < parlist.Count(); i++)
                        {
                            dxmb.sms_temp = dxmb.sms_temp.Replace(parlist[i].sms_parm, parmlist[i]);
                        }
                        SendSMSByTelPhone(dxmb.sms_temp, item.tel);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SendMSg()
        {
            try
            {
                var sendlist = djtjservice.Check();
                var djlist = sendlist.Where(t => t.cklx == "待机").ToList();
                var tjlist = sendlist.Where(t => t.cklx == "脱机").ToList();
                //待机超2小时
                var dj2hlist = sendlist.Where(t => t.cklx == "待机" && t.djsc >=120m).ToList();
                //脱机超2小时
                var tj2hlist = sendlist.Where(t => t.cklx == "脱机" && t.tjsc >= 120m).ToList();
                string djjt = string.Empty;
                string tjjt = string.Empty;
                string djjt2h = string.Empty;
                string tjjt2h = string.Empty;
                //要发送的内容
                string send_context = string.Empty;
                string send2h_content = string.Empty;
                djlist.ForEach(t =>
                {
                    djjt = djjt + t.sbqy + "、";
                });
                tjlist.ForEach(t =>
                {
                    tjjt = tjjt + t.sbqy + "、";
                });
                //超过2小时待机机台
                dj2hlist.ForEach(t =>
                {
                    djjt2h = djjt2h + t.sbqy + "、";
                });
                //超过2小时脱机机台
                tj2hlist.ForEach(t =>
                {
                    tjjt2h = tjjt2h + t.sbqy + "、";
                });
                
                //
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
                    parmlist.Add(DateTime.Now.ToString("MM-dd HH:mm:ss"));

                    //获取发送短信对象列表
                    var fsdxlist = djtjservice.Get_SmsList().Where(t => t.state == 1 && t.fl == 0);
                    SendSmsFromTel(parmlist, fsdxlist);
                }
                //超过2小时待机，停机机台
                if (!string.IsNullOrEmpty(djjt2h) || !string.IsNullOrEmpty(tjjt2h))
                {
                    List<string> parmlist = new List<string>();
                    if (!string.IsNullOrEmpty(djjt2h))
                    {
                        djjt2h = djjt2h.Remove(djjt2h.Length - 1);
                    }
                    else
                    {
                        djjt2h = "无";
                    }
                    if (!string.IsNullOrEmpty(tjjt2h))
                    {
                        tjjt2h = tjjt2h.Remove(tjjt2h.Length - 1);
                    }
                    else
                    {
                        tjjt2h = "无";
                    }
                    parmlist.Add(djjt2h);
                    parmlist.Add(tjjt2h);
                    parmlist.Add(DateTime.Now.ToString("MM-dd HH:mm:ss"));
                    //获取发送短信对象列表
                    var fsdxlist = djtjservice.Get_SmsList().Where(t => t.state == 1 && t.fl==1);
                    SendSmsFromTel(parmlist, fsdxlist);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        private void SendSMSByTelPhone(string send_context,string tel)
        {
            //判断模板里的参数是否被替换
            if (send_context.IndexOf("{") == -1)
            {
                var ret = Save_To_MidTable(new sys_sms()
                {
                    context = send_context,
                    tel = tel
                });
                //保存成功
                if (ret > 0)
                {
                    djtjservice.SaveToLog(new base_sms_log()
                    {
                        tel = tel,
                        content = send_context,
                        sendtime = DateTime.Now
                    });
                }
            }
        }

        private void SendSmsFromTel(List<string> parmlist, IEnumerable<base_sms> fsdxlist)
        {
            string send_context = string.Empty;
            foreach (var item in fsdxlist)
            {
                send_context = string.Empty;
                //短信模板
                var dxmb = djtjservice.Get_SMS_Temp(item.sms_id);
                //短信模板参数
                var parlist = djtjservice.Get_Temp_Parm(item.sms_id).OrderBy(t => t.id).ToList();
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
}
