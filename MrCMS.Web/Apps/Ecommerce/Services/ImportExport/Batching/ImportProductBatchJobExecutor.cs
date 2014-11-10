using System;
using MrCMS.Batching;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Events.Documents;
using MrCMS.Search;
using MrCMS.Services;
using MrCMS.Services.Notifications;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Batching
{
    public class ImportProductBatchJobExecutor : BaseBatchJobExecutor<ImportProductBatchJob>
    {
        private readonly IImportProductsService _importProductsService;

        public ImportProductBatchJobExecutor(ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus, IImportProductsService importProductsService)
            : base(setBatchJobJobExecutionStatus)
        {
            _importProductsService = importProductsService;
        }

        protected override BatchJobExecutionResult OnExecute(ImportProductBatchJob batchJob)
        {
            using (EventContext.Instance.Disable<IOnTransientNotificationPublished>())
            using (EventContext.Instance.Disable<IOnPersistentNotificationPublished>())
            using (EventContext.Instance.Disable<UpdateIndicesListener>())
            using (EventContext.Instance.Disable<UpdateUniversalSearch>())
            using (EventContext.Instance.Disable<WebpageUpdatedNotification>())
            using (EventContext.Instance.Disable<DocumentAddedNotification>())
            using (EventContext.Instance.Disable<MediaCategoryUpdatedNotification>())
            {
                try
                {
                    _importProductsService.ImportProduct(batchJob.DTO);
                    return BatchJobExecutionResult.Success();
                }
                catch (Exception exception)
                {
                    CurrentRequestData.ErrorSignal.Raise(exception);
                    return BatchJobExecutionResult.Failure(exception.Message);
                }
            }
        }
    }
}