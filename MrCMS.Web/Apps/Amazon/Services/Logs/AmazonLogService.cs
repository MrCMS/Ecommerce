using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Logs;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Misc;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Amazon.Services.Logs
{
    public class AmazonLogService : IAmazonLogService
    {
        private readonly ISession _session;
        private readonly IAmazonSessionManager _amazonSessionManager;

        public AmazonLogService(ISession session, IAmazonSessionManager amazonSessionManager)
        {
            _session = session;
            _amazonSessionManager = amazonSessionManager;
        }


        public void Sync()
        {
            _amazonSessionManager.SetSessionValue("amazon-progress",new AmazonProgressModel());
            UpdateProgressBarStatus("Initiation",100,1);
            for (var i = 0; i < 100; i++)
            {
                Thread.Sleep(1000);
                UpdateProgressBarStatus("Stage", null, i+1);
            }
            UpdateProgressBarStatus("Completion", 100, 100);
        }

        private AmazonProgressModel Progress
        {
            get
            {
                if (_amazonSessionManager.GetSessionValue<AmazonProgressModel>("amazon-progress")!=null)
                    return _amazonSessionManager.GetSessionValue<AmazonProgressModel>("amazon-progress");

                _amazonSessionManager.SetSessionValue("amazon-progress", new AmazonProgressModel());
                return new AmazonProgressModel();
            }
            set { _amazonSessionManager.SetSessionValue("amazon-progress", value); }
        }

        public void UpdateProgressBarStatus(string status, int? totalRecords,
            int? processedRecords)
        {
            var progress = Progress;
            if (!String.IsNullOrWhiteSpace(status))
            {
                progress.Statuses.Add(status);
                progress.CurrentStatus = status;
            }
            if (totalRecords.HasValue)
                progress.TotalActions = totalRecords.Value;
            if (processedRecords.HasValue)
                progress.ProcessedActions = processedRecords.Value;

            Progress = progress;
        }

        public object GetProgressBarStatus()
        {
            var progress = Progress;
            return new
            {
                PercentComplete = progress.PercentComplete,
                CurrentStatus = progress.CurrentStatus,
                Statuses = progress.Statuses
            };
        }










        private AmazonLog Add(AmazonLog log)
        {
            _session.Transact(session => session.Save(log));
            return log;
        }

        public AmazonLog Add(AmazonLogType type, AmazonLogStatus status, AmazonApiSection? apiSection, 
            string apiOperation, string message, string details)
        {
            var log = new AmazonLog()
                {
                    Type = type,
                    Status = status,
                    ApiSection = apiSection,
                    ApiOperation = apiOperation,
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

        public void DeleteAllLogs()
        {
            _session.CreateQuery("DELETE FROM Amazon_AmazonLog").ExecuteUpdate();
        }

        public void DeleteLog(AmazonLog log)
        {
            _session.Transact(session => session.Delete(log));
        }
    }
}