using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LBJOEE.Models;
using log4net;
using Dapper;
namespace LBJOEE.Services
{
    public class EventLogService: DBImp<sys_eventlog>
    {
        private ILog log;
        public EventLogService()
        {
            log = LogManager.GetLogger(this.GetType());
        }
        public void Save_EventLog(base_sbxx sbxx)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into sys_eventlog ");
                sql.Append("(sbbh, sbqy, rq, message,eventid) ");
                sql.Append(" select :sbbh, :sbqy, :rq, :message,:eventid ");
                sql.Append(" FROM   dual ");
                sql.Append(" where  not exists(select * ");
                sql.Append(" from sys_eventlog ");
                sql.Append(" where sbbh = :sbbh ");
                sql.Append(" and    rq = :rq) ");
                EventLogEntryCollection eventCollection;
                EventLog systemEvent = new EventLog();
                systemEvent.Log = "System";
                eventCollection = systemEvent.Entries;
                List<sys_eventlog> eventlogs = new List<sys_eventlog>();
                for (int i = 0; i < eventCollection.Count; i++)
                {
                    EventLogEntry entry = eventCollection[i];
                    if (entry.InstanceId == 41 || entry.InstanceId == 2147489656)
                    {
                        sys_eventlog eventlog = new sys_eventlog()
                        {
                            message = entry.Message,
                            rq = entry.TimeGenerated,
                            sbbh = sbxx.sbbh,
                            sbqy = sbxx.sbqy,
                            eventid = entry.InstanceId
                        };
                        eventlogs.Add(eventlog);
                    }
                }
                var ret = Db.Connection.Execute(sql.ToString(), eventlogs);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
        }
    }
}
