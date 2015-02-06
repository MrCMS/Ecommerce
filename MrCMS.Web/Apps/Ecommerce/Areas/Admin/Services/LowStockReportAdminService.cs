using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class LowStockReportAdminService : ILowStockReportAdminService
    {
        private readonly IGetLowStockQuery _getLowStockQuery;
        private readonly IGetStockReportFile _getStockReportFile;
        private readonly IStringResourceProvider _stringResourceProvider;

        public LowStockReportAdminService(IGetLowStockQuery getLowStockQuery,
            IStringResourceProvider stringResourceProvider,
            IGetStockReportFile getStockReportFile)
        {
            _getLowStockQuery = getLowStockQuery;
            _stringResourceProvider = stringResourceProvider;
            _getStockReportFile = getStockReportFile;
        }

        public IPagedList<ProductVariant> Search(LowStockReportSearchModel searchModel)
        {
            IQueryable<ProductVariant> queryOver = _getLowStockQuery.Get(searchModel);

            return queryOver.Paged(searchModel.Page);
        }

        public ExportStockReportResult ExportLowStockReport(LowStockReportSearchModel searchModel)
        {
            try
            {
                List<ProductVariant> items = _getLowStockQuery.Get(searchModel).ToList();
                return new ExportStockReportResult
                {
                    FileResult = GetLowStockFileResult(_getStockReportFile.GetFile(items)),
                    Success = true
                };
            }
            catch (Exception exception)
            {
                CurrentRequestData.ErrorSignal.Raise(exception);
                return new ExportStockReportResult
                {
                    Message =
                        _stringResourceProvider.GetValue("Export Low Stock Report Failed",
                            "Low Stock Report exporting has failed. Please try again and contact system administration if error continues to appear.")
                };
            }
        }

        private FileResult GetLowStockFileResult(byte[] file)
        {
            return new FileContentResult(file, "text/csv")
            {
                FileDownloadName = string.Format("mrcms-low-stock-report-{0:yyyy-MM-dd-HH-mm-ss}.csv", DateTime.UtcNow)
            };
        }
    }
}