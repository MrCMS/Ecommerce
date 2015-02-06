using System;
using MrCMS.Batching;
using MrCMS.Services.Notifications;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Batching
{
    public class ImportProductBatchJobExecutor : BaseBatchJobExecutor<ImportProductBatchJob>
    {
        private readonly IImportProductsService _importProductsService;

        public ImportProductBatchJobExecutor(ISetBatchJobExecutionStatus setBatchJobJobExecutionStatus,
            IImportProductsService importProductsService)
            : base(setBatchJobJobExecutionStatus)
        {
            _importProductsService = importProductsService;
        }

        protected override BatchJobExecutionResult OnExecute(ImportProductBatchJob batchJob)
        {
            using (new NotificationDisabler())
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