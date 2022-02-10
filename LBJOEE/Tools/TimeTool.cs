using LBJOEE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE.Tools
{
    public static class TimeTool
    {
        public static sys_bcxx GetBCInfo(DateTime input_rq)
        {
            try
            {
                DateTime d1 = System.Convert.ToDateTime(input_rq.ToString("yyyy-MM-dd 08:00:00"));
                DateTime d2 = System.Convert.ToDateTime(input_rq.ToString("yyyy-MM-dd 20:00:00"));
                DateTime d3 = d2.AddHours(12);
                DateTime d0 = d1.AddHours(-12);
                DateTime dt_ksrq = System.Convert.ToDateTime(null);
                DateTime dt_jsrq = System.Convert.ToDateTime(null);
                sys_bcxx bcxx = new sys_bcxx();
                //判断时间所在班次
                if (DateTime.Compare(d0, input_rq) <= 0 && DateTime.Compare(input_rq, d1) <= 0)
                {
                    bcxx.kssj = d0;
                    bcxx.jssj = d1;
                    bcxx.up_kssj = d0.AddHours(-12);
                    bcxx.up_jssj = d0;
                    bcxx.next_kssj = d1;
                    bcxx.next_jssj = d1.AddHours(12);
                }
                else if (DateTime.Compare(d1, input_rq) <= 0 && DateTime.Compare(input_rq, d2) <= 0)
                {
                    bcxx.kssj = d1;
                    bcxx.jssj = d2;
                    bcxx.up_kssj = d1.AddHours(-12);
                    bcxx.up_jssj = d1;
                    bcxx.next_kssj = d2;
                    bcxx.next_jssj = d2.AddHours(12);
                }
                else if (DateTime.Compare(d2, input_rq) <= 0 && DateTime.Compare(input_rq, d3) <= 0)
                {
                    bcxx.kssj = d2;
                    bcxx.jssj = d3;
                    bcxx.up_kssj = d2.AddHours(-12);
                    bcxx.up_jssj = d2;
                    bcxx.next_kssj = d3;
                    bcxx.next_jssj = d3.AddHours(12);
                }
                //设置班次名称
                if (bcxx.kssj.Hour == 20)
                {
                    bcxx.bcmc = "夜班";
                }
                if (bcxx.kssj.Hour == 8)
                {
                    bcxx.bcmc = "白班";
                }
                if (bcxx.up_kssj.Hour == 20)
                {
                    bcxx.up_bcmc = "夜班";
                }
                if (bcxx.up_kssj.Hour == 8)
                {
                    bcxx.up_bcmc = "白班";
                }
                if (bcxx.next_kssj.Hour == 20)
                {
                    bcxx.next_bcmc = "夜班";
                }
                if (bcxx.next_kssj.Hour == 8)
                {
                    bcxx.next_bcmc = "白班";
                }
                return bcxx;
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("OEECalc.Tool.TimeTool").Error(e.Message);
                return new sys_bcxx();
            }
        }
        public static bool IsSameBC(sbtj tjinfo)
        {
            try
            {
                var bcxx = GetBCInfo(tjinfo.tjkssj);
                if (DateTime.Compare(bcxx.kssj, tjinfo.tjkssj) <= 0 && DateTime.Compare(tjinfo.tjkssj, bcxx.jssj) <= 0 &&
                    DateTime.Compare(bcxx.kssj, tjinfo.tjjssj) <= 0 && DateTime.Compare(tjinfo.tjjssj, bcxx.jssj) <= 0
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("OEECalc.Tool.TimeTool").Error(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 完成停机时段，停机时间分解到每个班次上
        /// </summary>
        /// <param name="tjinfo"></param>
        /// <returns></returns>
        public static IEnumerable<sbtj> Calc_SBTJSD(sbtj tjinfo)
        {
            try
            {
                List<sbtj> list = new List<sbtj>();
                DateTime kstjsj = tjinfo.tjkssj;
                DateTime jstjsj = tjinfo.tjjssj;
                DateTime dt_ksrq = System.Convert.ToDateTime(null);
                DateTime dt_jsrq = System.Convert.ToDateTime(null);
                DateTime current_sj = System.Convert.ToDateTime(null);
                current_sj = TimeTool.GetBCInfo(kstjsj).kssj;
                while (DateTime.Compare(current_sj, jstjsj) <= 0)
                {
                    DateTime next_sj = current_sj.AddHours(12);
                    //开始停机时间在班次范围内，结束时间不再班次范围内
                    if (DateTime.Compare(current_sj, kstjsj) <= 0 && DateTime.Compare(kstjsj, next_sj) <= 0 &&
                        DateTime.Compare(next_sj, jstjsj) < 0
                        )
                    {
                        dt_ksrq = kstjsj;
                        dt_jsrq = next_sj;
                    }//开始停机时间不再班次范围内，结束停机时间在班次范围内
                    else if (DateTime.Compare(current_sj, jstjsj) <= 0 && DateTime.Compare(jstjsj, next_sj) <= 0 &&
                        DateTime.Compare(kstjsj, current_sj) < 0
                        )
                    {
                        dt_ksrq = current_sj;
                        dt_jsrq = jstjsj;
                    }//开始停机时间，结束停机时间都在班次范围内
                    else if (DateTime.Compare(current_sj, kstjsj) <= 0 && DateTime.Compare(kstjsj, next_sj) <= 0 &&
                        DateTime.Compare(current_sj, jstjsj) <= 0 && DateTime.Compare(jstjsj, next_sj) <= 0
                        )
                    {
                        dt_ksrq = kstjsj;
                        dt_jsrq = jstjsj;
                    }
                    else
                    {
                        dt_ksrq = current_sj;
                        dt_jsrq = next_sj;
                    }
                    list.Add(new sbtj()
                    {
                        sbbh = tjinfo.sbbh,
                        tjkssj = dt_ksrq,
                        tjjssj = dt_jsrq,
                        tjsj = System.Convert.ToInt32((dt_jsrq - dt_ksrq).TotalSeconds),
                        tjlx = tjinfo.tjlx,
                        tjms = tjinfo.tjms,
                        lx = "1",
                    });
                    current_sj = next_sj;
                }
                return list;
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("LBJOEE.Tools.TimeTool").Error(e.Message);
                return new List<sbtj>();
            }
        }
    }
}
