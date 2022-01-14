using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LBJOEE.Models;
using Dapper;
using System.Data;
using LBJOEE.Tools;
using log4net;
namespace LBJOEE.Services
{
    /// <summary>
    /// 设备数据服务类
    /// </summary>
    public class SBSJService:DBImp<sjcjnew>
    {
        private static SBSJService instance = null;
        private static readonly object padlock = new object();
        private ILog log = null;
        private SBSJService()
        {
            log = LogManager.GetLogger(this.GetType());
        }

        public static SBSJService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new SBSJService();
                        }
                    }
                }
                return instance;
            }
        }

        public void SaveOriginalData(originaldata entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into receivedata ");
                sql.Append(" (rq, sbbh, ip, json) ");
                sql.Append(" values ");
                sql.Append(" (sysdate, :sbbh, :ip, :json) ");
                DynamicParameters p = new DynamicParameters();
                p.Add(":sbbh", entity.sbbh, DbType.String, ParameterDirection.Input);
                p.Add(":ip", entity.ip, DbType.String, ParameterDirection.Input);
                p.Add(":json", entity.json, DbType.String, ParameterDirection.Input);
                if (Tool.IsPing())
                {
                    Db.Connection.Execute(sql.ToString(), p);
                }
                else
                {
                    DataBackUp.SaveOrginalDataToLocal(entity);
                }
            }
            catch (Exception)
            {
                //Environment.Exit(0);
            }
        }
        /// <summary>
        /// 保存掉网时的数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddByDate(sjcjnew entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into sjcj ");
                sql.Append("(cjsj, sbbh, sbip, jp, jgs, yssd, jysj, jssj, lbhd, zyyl, yxzt, bjzt, xhsj, sdzt, zdzt, jt, ljkjsj, dzcdqwz, dqyl, dqll, ksyl, essd, sssd, sdqd, tqxc, tcsj, lbwz, gswz, mysd, gssd, zzyl, ysyl, ysll, zltx, hml, jzsyhd, ysxc, xqctsj, zzysj, sscnylsjz, zycnylsjz, smqdyl, smqdll, smqdwz, smksyl, smksll, smkswz, skdyyl, skdyll, smdywz, smgyyl, smgyll, smgywz, kssmsd, mssmsd, kmhcyl, kmhcll, kmhcwz, kmkyyl, kmksll, kmkswz, kmgyyl, kmgyll, kmgywz, mskmsd, kskmsd, cx1jryl, cx1jrll, cx1htyl, cx1htll, cx2jryl, cx2jrll, cx2htyl, cx2htll, cxrys, cxcys, cxcndqyl, cxcnyltlz, cxcnylsdz, kyyswz, kyeswz, kysswz, kyzywz, kygcwz, dqylsd, dqllsd, dqyssd, dhylsd, dhllsd, dhyssd, dzcs, kscnyl, zycnyl, hchcwz, kmzzwz, mjwz, yw, mswz, mjtcjs, yasxc, jzhs, zhls, gsks, gsqj, sysj, rmjcs) ");
                sql.Append(" values ");
                sql.Append(" (:cjsj, :sbbh, :sbip, :jp, :jgs, :yssd, :jysj, :jssj, :lbhd, :zyyl,  :yxzt, :bjzt, :xhsj, :sdzt, :zdzt, :jt, :ljkjsj, :dzcdqwz, :dqyl, :dqll, :ksyl, :essd, :sssd, :sdqd, :tqxc, :tcsj, :lbwz, :gswz, :mysd, :gssd, :zzyl, :ysyl, :ysll, :zltx, :hml, :jzsyhd, :ysxc, :xqctsj, :zzysj, :sscnylsjz, :zycnylsjz, :smqdyl, :smqdll, :smqdwz, :smksyl, :smksll, :smkswz, :skdyyl, :skdyll, :smdywz, :smgyyl, :smgyll, :smgywz, :kssmsd, :mssmsd, :kmhcyl, :kmhcll, :kmhcwz, :kmkyyl, :kmksll, :kmkswz, :kmgyyl, :kmgyll, :kmgywz, :mskmsd, :kskmsd, :cx1jryl, :cx1jrll, :cx1htyl, :cx1htll, :cx2jryl, :cx2jrll, :cx2htyl, :cx2htll, :cxrys, :cxcys, :cxcndqyl, :cxcnyltlz, :cxcnylsdz, :kyyswz, :kyeswz, :kysswz, :kyzywz, :kygcwz, :dqylsd, :dqllsd, :dqyssd, :dhylsd, :dhllsd, :dhyssd, :dzcs, :kscnyl, :zycnyl, :hchcwz, :kmzzwz, :mjwz, :yw, :mswz, :mjtcjs, :yasxc, :jzhs, :zhls, :gsks, :gsqj, :sysj, :rmjcs)");
                return Db.Connection.Execute(sql.ToString(), entity);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0;
            }
        }

        public int TJSJCJ(sjcjnew entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into tjsjcj ");
                sql.Append("(cjsj, sbbh, sbip, jp, jgs, yssd, jysj, jssj, lbhd, zyyl, yxzt, bjzt, xhsj, sdzt, zdzt, jt, ljkjsj, dzcdqwz, dqyl, dqll, ksyl, essd, sssd, sdqd, tqxc, tcsj, lbwz, gswz, mysd, gssd, zzyl, ysyl, ysll, zltx, hml, jzsyhd, ysxc, xqctsj, zzysj, sscnylsjz, zycnylsjz, smqdyl, smqdll, smqdwz, smksyl, smksll, smkswz, skdyyl, skdyll, smdywz, smgyyl, smgyll, smgywz, kssmsd, mssmsd, kmhcyl, kmhcll, kmhcwz, kmkyyl, kmksll, kmkswz, kmgyyl, kmgyll, kmgywz, mskmsd, kskmsd, cx1jryl, cx1jrll, cx1htyl, cx1htll, cx2jryl, cx2jrll, cx2htyl, cx2htll, cxrys, cxcys, cxcndqyl, cxcnyltlz, cxcnylsdz, kyyswz, kyeswz, kysswz, kyzywz, kygcwz, dqylsd, dqllsd, dqyssd, dhylsd, dhllsd, dhyssd, dzcs, kscnyl, zycnyl, hchcwz, kmzzwz, mjwz, yw, mswz, mjtcjs, yasxc, jzhs, zhls, gsks, gsqj, sysj, rmjcs) ");
                sql.Append(" values ");
                sql.Append(" (sysdate, :sbbh, :sbip, :jp, :jgs, :yssd, :jysj, :jssj, :lbhd, :zyyl,  :yxzt, :bjzt, :xhsj, :sdzt, :zdzt, :jt, :ljkjsj, :dzcdqwz, :dqyl, :dqll, :ksyl, :essd, :sssd, :sdqd, :tqxc, :tcsj, :lbwz, :gswz, :mysd, :gssd, :zzyl, :ysyl, :ysll, :zltx, :hml, :jzsyhd, :ysxc, :xqctsj, :zzysj, :sscnylsjz, :zycnylsjz, :smqdyl, :smqdll, :smqdwz, :smksyl, :smksll, :smkswz, :skdyyl, :skdyll, :smdywz, :smgyyl, :smgyll, :smgywz, :kssmsd, :mssmsd, :kmhcyl, :kmhcll, :kmhcwz, :kmkyyl, :kmksll, :kmkswz, :kmgyyl, :kmgyll, :kmgywz, :mskmsd, :kskmsd, :cx1jryl, :cx1jrll, :cx1htyl, :cx1htll, :cx2jryl, :cx2jrll, :cx2htyl, :cx2htll, :cxrys, :cxcys, :cxcndqyl, :cxcnyltlz, :cxcnylsdz, :kyyswz, :kyeswz, :kysswz, :kyzywz, :kygcwz, :dqylsd, :dqllsd, :dqyssd, :dhylsd, :dhllsd, :dhyssd, :dzcs, :kscnyl, :zycnyl, :hchcwz, :kmzzwz, :mjwz, :yw, :mswz, :mjtcjs, :yasxc, :jzhs, :zhls, :gsks, :gsqj, :sysj, :rmjcs)");
                return Db.Connection.Execute(sql.ToString(), entity);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0;
            }
        }
        /// <summary>
        /// 获取非运行状态下，上传的数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<sjcjnew> Get_NoRunList(string sbbh,int interval=7)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select rowid as rid,cjsj, sbbh, sbip, jp, jgs, yssd, jysj, jssj, lbhd, zyyl, yxzt, bjzt, xhsj, sdzt, zdzt, jt, ljkjsj, dzcdqwz, dqyl, dqll, ksyl, essd, sssd, sdqd, tqxc, tcsj, lbwz, gswz, mysd, gssd, zzyl, ysyl, ysll, zltx, hml, jzsyhd, ysxc, xqctsj, zzysj, sscnylsjz, zycnylsjz, smqdyl, smqdll, smqdwz, smksyl, smksll, smkswz, skdyyl, skdyll, smdywz, smgyyl, smgyll, smgywz, kssmsd, mssmsd, kmhcyl, kmhcll, kmhcwz, kmkyyl, kmksll, kmkswz, kmgyyl, kmgyll, kmgywz, mskmsd, kskmsd, cx1jryl, cx1jrll, cx1htyl, cx1htll, cx2jryl, cx2jrll, cx2htyl, cx2htll, cxrys, cxcys, cxcndqyl, cxcnyltlz, cxcnylsdz, kyyswz, kyeswz, kysswz, kyzywz, kygcwz, dqylsd, dqllsd, dqyssd, dhylsd, dhllsd, dhyssd, dzcs, kscnyl, zycnyl, hchcwz, kmzzwz, mjwz, yw, mswz, mjtcjs, yasxc, jzhs, zhls, gsks, gsqj, sysj, rmjcs ");
                sql.Append(" FROM   tjsjcj ");
                sql.Append(" where  cjsj between sysdate - (1 / (24*60)) * :interval and sysdate ");
                sql.Append(" and    sbbh = :sbbh ");
                return Db.Connection.Query<sjcjnew>(sql.ToString(), new { sbbh = sbbh, interval=interval });
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<sjcjnew>();
            }
        }

        public bool DelTjsjcj(string rid)
        {
            try
            {
               var cnt = Db.Connection.Execute("delete from tjsjcj where rowid = :rid ",new { rid=rid});
                return cnt > 0 ? true : false;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
        }

        public IEnumerable<base_sbxx_conf> Get_Conf_BySBBH(string sbbh)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select sbbh,confkey,confval FROM  base_sbxx_conf where sbbh=:sbbh ");
                return Db.Connection.Query<base_sbxx_conf>(sql.ToString(), new { sbbh = sbbh });
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<base_sbxx_conf>();
            }
        }
    }
}
