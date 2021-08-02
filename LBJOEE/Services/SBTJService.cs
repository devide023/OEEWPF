using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.Services
{
    /// <summary>
    /// 设备停机时间服务
    /// </summary>
    public class SBTJService:DBImp<sbtj>
    {
        /// <summary>
        /// 计算设备停机时间
        /// </summary>
        /// <returns></returns>
        public bool CalcTjsj()
        {
            SBXXService sbxxservice = new SBXXService();
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
    }
}
