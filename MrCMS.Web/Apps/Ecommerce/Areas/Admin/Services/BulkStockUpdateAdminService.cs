using System;
using System.IO;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class BulkStockUpdateAdminService : IBulkStockUpdateAdminService
    {
        private readonly IBulkSimpleStockUpdateService _bulkSimpleStockUpdateService;
        private readonly IBulkWarehousedStockUpdateService _bulkWarehousedStockUpdateService;
        private readonly EcommerceSettings _ecommerceSettings;

        public BulkStockUpdateAdminService(EcommerceSettings ecommerceSettings,
            IBulkSimpleStockUpdateService bulkSimpleStockUpdateService,
            IBulkWarehousedStockUpdateService bulkWarehousedStockUpdateService)
        {
            _bulkSimpleStockUpdateService = bulkSimpleStockUpdateService;
            _ecommerceSettings = ecommerceSettings;
            _bulkWarehousedStockUpdateService = bulkWarehousedStockUpdateService;
        }

        public BulkStockUpdateResult BulkStockUpdate(Stream file)
        {
            return _ecommerceSettings.WarehouseStockEnabled
                ? _bulkWarehousedStockUpdateService.Update(file)
                : _bulkSimpleStockUpdateService.Update(file);
        }
    }
}