using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using MrCMS.Website;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class StockReportAdminService : IStockReportAdminService
    {
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IGetStockExportData _getStockExportData;

        public StockReportAdminService(
            IStringResourceProvider stringResourceProvider, EcommerceSettings ecommerceSettings,
            IGetStockExportData getStockExportData)
        {
            _stringResourceProvider = stringResourceProvider;
            _ecommerceSettings = ecommerceSettings;
            _getStockExportData = getStockExportData;
        }

        public ExportStockReportResult ExportStockReport()
        {
            try
            {
                byte[] fileData = _ecommerceSettings.WarehouseStockEnabled
                    ? _getStockExportData.GetWarehousedExport()
                    : _getStockExportData.GetStandardExport();
                return new ExportStockReportResult
                {
                    FileResult = GetStockFileResult(fileData),
                    Success = true
                };
            }
            catch (Exception exception)
            {
                CurrentRequestData.ErrorSignal.Raise(exception);
                return new ExportStockReportResult
                {
                    Message =
                        _stringResourceProvider.GetValue("Export Stock Report Failed",
                            "Stock Report exporting has failed. Please try again and contact system administration if an error continues to appear.")
                };
            }
        }

        private FileResult GetStockFileResult(byte[] file)
        {
            var warehoused = _ecommerceSettings.WarehouseStockEnabled ? "warehoused-" : string.Empty;
            return new FileContentResult(file, "text/csv")
            {
                FileDownloadName = string.Format("mrcms-{0}stock-report-{1:yyyy-MM-dd-HH-mm-ss}.csv", warehoused, DateTime.UtcNow)
            };
        }
    }

    public interface IGetStockExportData
    {
        byte[] GetStandardExport();
        byte[] GetWarehousedExport();
    }

    public class GetStockExportData : IGetStockExportData
    {
        private readonly IGetSimpleStockExportData _getSimpleStockExportData;
        private readonly IGetWarehousedStockExportData _getWarehousedStockExportData;

        public GetStockExportData(IGetSimpleStockExportData getSimpleStockExportData, IGetWarehousedStockExportData getWarehousedStockExportData)
        {
            _getSimpleStockExportData = getSimpleStockExportData;
            _getWarehousedStockExportData = getWarehousedStockExportData;
        }

        public byte[] GetStandardExport()
        {
            return _getSimpleStockExportData.GetStockData();
        }

        public byte[] GetWarehousedExport()
        {
            return _getWarehousedStockExportData.GetStockData();
        }
    }

    public interface IGetWarehousedStockExportData
    {
        byte[] GetStockData();
    }

    public class GetWarehousedStockExportData : IGetWarehousedStockExportData
    {
        private readonly ISession _session;
        private readonly IGetWarehousedStockReportFile _getWarehousedStockReportFile;

        public GetWarehousedStockExportData(ISession session, IGetWarehousedStockReportFile getWarehousedStockReportFile)
        {
            _session = session;
            _getWarehousedStockReportFile = getWarehousedStockReportFile;
        }

        public byte[] GetStockData()
        {
            IList<WarehouseStock> items =
                _session.Query<WarehouseStock>()
                    .Where(stock => stock.ProductVariant.TrackingPolicy == TrackingPolicy.Track)
                    .Fetch(stock => stock.ProductVariant)
                    .Fetch(stock => stock.Warehouse)
                    .Cacheable().ToList()
                    .OrderBy(stock => stock.ProductVariant.SKU)
                    .ThenBy(stock => stock.Warehouse.Id).ToList();

            return _getWarehousedStockReportFile.GetFile(items);
        }
    }

    public interface IGetWarehousedStockReportFile
    {
        byte[] GetFile(IList<WarehouseStock> items);
    }

    public class GetWarehousedStockReportFile : IGetWarehousedStockReportFile
    {
        private readonly ICSVFileWriter _csvFileWriter;

        public GetWarehousedStockReportFile(ICSVFileWriter csvFileWriter)
        {
            _csvFileWriter = csvFileWriter;
        }

        public byte[] GetFile(IList<WarehouseStock> items)
        {
            var sortedDictionary = new Dictionary<string, Func<WarehouseStock, object>>
            {
                {"Name", stock => stock.ProductVariantName},
                {"SKU", stock => stock.ProductVariantSKU},
                {"Warehouse", stock => stock.Warehouse.Id},
                {"Stock Remaining", stock => stock.StockLevel}
            };
            return _csvFileWriter.GetFile(items, sortedDictionary);
        }
    }

    public interface IGetSimpleStockExportData
    {
        byte[] GetStockData();
    }

    public class GetSimpleStockExportData : IGetSimpleStockExportData
    {
        private readonly ISession _session;
        private readonly IGetStockReportFile _getStockReportFile;

        public GetSimpleStockExportData(ISession session, IGetStockReportFile getStockReportFile)
        {
            _session = session;
            _getStockReportFile = getStockReportFile;
        }

        public byte[] GetStockData()
        {
            IList<ProductVariant> items =
                        _session.QueryOver<ProductVariant>()
                            .Where(variant => variant.TrackingPolicy == TrackingPolicy.Track)
                            .Cacheable().List();
            return _getStockReportFile.GetFile(items);
        }
    }
}