using System.IO;
using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class ImportDummyProducts : IImportDummyProducts
    {
        private readonly IImportProductsManager _importExportManager;
        private readonly ISynchronousBatchRunExecution _synchronousBatchRunExecution;

        public ImportDummyProducts(IImportProductsManager importExportManager, ISynchronousBatchRunExecution synchronousBatchRunExecution)
        {
            _importExportManager = importExportManager;
            _synchronousBatchRunExecution = synchronousBatchRunExecution;
        }

        public void Import()
        {
            var memoryStream = new MemoryStream(EcommerceInstallHelper.GetFileFromUrl(EcommerceInstallInfo.ProductsExcelUrl));
            var result = _importExportManager.ImportProductsFromExcel(memoryStream, autoStartBatch: false);

            var batchRun = result.Batch.BatchRuns.First();
            _synchronousBatchRunExecution.Execute(batchRun);
        }
    }
}