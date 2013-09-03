using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Logs;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Amazon.Services.Logs
{
    public class AmazonLogService : IAmazonLogService
    {
        private readonly ISession _session;

        public AmazonLogService(ISession session)
        {
            _session = session;
        }

        public AmazonLog Add(AmazonLog log)
        {
            _session.Transact(session => session.Save(log));
            return log;
        }

        public AmazonLog Add(AmazonLogType type, AmazonLogStatus status, string message, string details)
        {
            var log = new AmazonLog()
                {
                    Type = type,
                    Status = status,
                    Guid = Guid.NewGuid(),
                    Message = message,
                    Details = details,
                    Site = CurrentRequestData.CurrentSite
                };
            _session.Transact(session => session.Save(log));
            return log;
        }

        public IList<AmazonLog> GetAllLogEntries()
        {
            return BaseQuery().Cacheable().List();
        }

        public void DeleteAllLogs()
        {
            _session.CreateQuery("DELETE FROM Amazon_Log").ExecuteUpdate();
        }

        public void DeleteLog(AmazonLog log)
        {
            _session.Transact(session => session.Delete(log));
        }

        public IPagedList<AmazonLog> GetEntriesPaged(int pageNum, AmazonLogType? type = null, AmazonLogStatus? status = null, int pageSize = 10)
        {
            var query = BaseQuery();
            if (type.HasValue)
                query = query.Where(log => log.Type == type);
            if (status.HasValue)
                query = query.Where(log => log.Status == status);
            return query.Paged(pageNum, pageSize);
        }

        private IQueryOver<AmazonLog, AmazonLog> BaseQuery()
        {
            return
                _session.QueryOver<AmazonLog>()
                        .Where(entry => entry.Site == CurrentRequestData.CurrentSite)
                        .OrderBy(entry => entry.Id)
                        .Desc;
        }
    }
}