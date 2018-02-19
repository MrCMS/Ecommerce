using System.IO;
using System.Linq;
using Iesi.Collections;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class ImportDummyCategories : IImportDummyCategories
    {
        private readonly ImportExportManager _importExportManager;
        private readonly IWebpageAdminService _webpageAdminService;
        private readonly IGetDocumentByUrl<Category> _getCategoryByUrl;
        private readonly ISynchronousBatchRunExecution _synchronousBatchRunExecution;

        public ImportDummyCategories(ImportExportManager importExportManager, IWebpageAdminService webpageAdminService, IGetDocumentByUrl<Category> getCategoryByUrl, ISynchronousBatchRunExecution synchronousBatchRunExecution)
        {
            _importExportManager = importExportManager;
            _webpageAdminService = webpageAdminService;
            _getCategoryByUrl = getCategoryByUrl;
            _synchronousBatchRunExecution = synchronousBatchRunExecution;
        }

        public void Import(MediaModel model)
        {
            var memoryStream = new MemoryStream(EcommerceInstallHelper.GetFileFromUrl(EcommerceInstallInfo.CategoryExcelUrl));
            var result = _importExportManager.ImportDocumentsFromExcel(memoryStream, false);
            
            var batchRun = result.Batch.BatchRuns.First();
            _synchronousBatchRunExecution.Execute(batchRun);

            SetFeaturedCategories(model);
        }

        private void SetFeaturedCategories(MediaModel model)
        {
            var cat1 = _getCategoryByUrl.GetByUrl(FeaturedCategoriesInfo.Category1Url);
            var cat2 = _getCategoryByUrl.GetByUrl(FeaturedCategoriesInfo.Category2Url);
            var cat3 = _getCategoryByUrl.GetByUrl(FeaturedCategoriesInfo.Category3Url);
            var cat4 = _getCategoryByUrl.GetByUrl(FeaturedCategoriesInfo.Category4Url);
            if (cat1 != null)
            {
                cat1.FeatureImage = model.FeaturedCategory1.FileUrl;
                _webpageAdminService.Add(cat1);
            }
            if (cat2 != null)
            {
                cat2.FeatureImage = model.FeaturedCategory2.FileUrl;
                _webpageAdminService.Add(cat2);
            }
            if (cat3 != null)
            {
                cat3.FeatureImage = model.FeaturedCategory3.FileUrl;
                _webpageAdminService.Add(cat3);
            }
            if (cat4 != null)
            {
                cat4.FeatureImage = model.FeaturedCategory4.FileUrl;
                _webpageAdminService.Add(cat4);
            }
        }
    }
}