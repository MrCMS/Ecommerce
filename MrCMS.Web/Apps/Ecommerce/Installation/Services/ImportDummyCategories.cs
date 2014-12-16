using System.IO;
using System.Linq;
using Iesi.Collections;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class ImportDummyCategories : IImportDummyCategories
    {
        private readonly ImportExportManager _importExportManager;
        private readonly IDocumentService _documentService;
        private readonly ISynchronousBatchRunExecution _synchronousBatchRunExecution;

        public ImportDummyCategories(ImportExportManager importExportManager, IDocumentService documentService, ISynchronousBatchRunExecution synchronousBatchRunExecution)
        {
            _importExportManager = importExportManager;
            _documentService = documentService;
            _synchronousBatchRunExecution = synchronousBatchRunExecution;
        }

        public void Import(MediaModel model)
        {
            var memoryStream = new MemoryStream(EcommerceInstallHelper.GetFileFromUrl(EcommerceInstallInfo.CategoryExcelUrl));
            var result = _importExportManager.ImportDocumentsFromExcel(memoryStream, false);
            
            var batchRun = result.Batch.BatchRuns.First();
            batchRun.Status = BatchRunStatus.Executing;
            _synchronousBatchRunExecution.Execute(batchRun);

            SetFeaturedProducts(model);
        }

        private void SetFeaturedProducts(MediaModel model)
        {
            var cat1 = _documentService.GetDocumentByUrl<Category>(FeaturedCategoriesInfo.Category1Url);
            var cat2 = _documentService.GetDocumentByUrl<Category>(FeaturedCategoriesInfo.Category2Url);
            var cat3 = _documentService.GetDocumentByUrl<Category>(FeaturedCategoriesInfo.Category3Url);
            var cat4 = _documentService.GetDocumentByUrl<Category>(FeaturedCategoriesInfo.Category4Url);
            if (cat1 != null)
            {
                cat1.FeatureImage = model.FeaturedCategory1.FileUrl;
                _documentService.SaveDocument(cat1);
            }
            if (cat2 != null)
            {
                cat2.FeatureImage = model.FeaturedCategory2.FileUrl;
                _documentService.SaveDocument(cat2);
            }
            if (cat3 != null)
            {
                cat3.FeatureImage = model.FeaturedCategory3.FileUrl;
                _documentService.SaveDocument(cat3);
            }
            if (cat4 != null)
            {
                cat4.FeatureImage = model.FeaturedCategory4.FileUrl;
                _documentService.SaveDocument(cat4);
            }
        }
    }
}