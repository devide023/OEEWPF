﻿using log4net;
using OEECalc.Model;
using OEECalc.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using DapperExtensions;
using DapperExtensions.Predicate;
using System.Threading.Tasks;
namespace OEECalc.Services
{
    public class SBCNService: OracleBaseFixture
    {
        private static SBCNService instance = null;
        private static readonly object padlock = new object();
        private SBXXService _sbxxservice;
        private TimeUtil _timeutil;
        private ILog log;
        private SBCNService()
        {
            log = LogManager.GetLogger(this.GetType());
            _sbxxservice = new SBXXService();
            _timeutil = new TimeUtil();
        }

        public static SBCNService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new SBCNService();
                        }
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// 设备日产能
        /// </summary>
        public void RiCn()
        {
            try
            {
                var sblist = _sbxxservice.Get_SBXX_List().OrderBy(t => t.sbqy).ToList();
                var servertime = _timeutil.ServerTime();
                PredicateGroup pg = new PredicateGroup()
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>()
                };
                foreach (var item in sblist)
                {
                    sbcntj cntj = new sbcntj();
                    cntj.sbbh = item.sbbh;
                    cntj.sj = Convert.ToDateTime(servertime.ToString("yyyy-MM-dd"));
                    cntj.jgs = get_rijgs(item.sbbh);
                    pg.Predicates.Clear();
                    pg.Predicates.Add(Predicates.Field<sbcntj>(f => f.sbbh, Operator.Eq, item.sbbh));
                    pg.Predicates.Add(Predicates.Field<sbcntj>(f => f.sj, Operator.Eq, cntj.sj));
                    var q = Db.GetList<sbcntj>(pg);
                    if (q.Count() == 0) { 
                    Db.Insert<sbcntj>(cntj);
                    }
                    else
                    {
                        var first = q.FirstOrDefault();
                        first.jgs = cntj.jgs;
                        Db.Update<sbcntj>(first);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }

        private long get_rijgs(string sbbh)
        {
            try
            {
                StringBuilder sqlmax = new StringBuilder();
                sqlmax.Append("select max(to_number(jgs)) maxjgs ");
                sqlmax.Append(" from sjcj ");
                sqlmax.Append(" where trunc(cjsj) = trunc(sysdate) ");
                sqlmax.Append(" and    trim(translate(jgs, '0123456789', ' ')) is NULL ");
                sqlmax.Append(" and sbbh = :sbbh ");
                StringBuilder sqlmin = new StringBuilder();
                sqlmin.Append("select min(to_number(jgs)) maxjgs ");
                sqlmin.Append(" from sjcj ");
                sqlmin.Append(" where trunc(cjsj) = trunc(sysdate) ");
                sqlmin.Append(" and    trim(translate(jgs, '0123456789', ' ')) is NULL ");
                sqlmin.Append(" and sbbh = :sbbh ");
                if (Tool.NetCheck.IsPing("172.16.201.175"))
                {
                    var max = Db.Connection.ExecuteScalar<long>(sqlmax.ToString(), new { sbbh = sbbh });
                    var min = Db.Connection.ExecuteScalar<long>(sqlmin.ToString(), new { sbbh = sbbh });
                    return max - min;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return 0;
            }
        }
    }
}