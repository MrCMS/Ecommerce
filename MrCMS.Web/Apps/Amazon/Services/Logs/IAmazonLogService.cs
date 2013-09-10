using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Logs;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Logs
{
    public interface IAmazonLogService
    {
        AmazonLog Add(AmazonLogType type, AmazonLogStatus status, AmazonApiSection? apiSection,
                      string apiOperation, string message, string details);

        IList<AmazonLog> GetAllLogEntries();
        IPagedList<AmazonLog> GetEntriesPaged(int pageNum, AmazonLogType? type = null,
                                                AmazonLogStatus? status = null, int pageSize = 10);
        void DeleteAllLogs();
        void DeleteLog(AmazonLog log);

        void Sync();

        void UpdateProgressBarStatus(string status, int? totalRecords,
                                            int? processedRecords);

        object GetProgressBarStatus();
    }
}