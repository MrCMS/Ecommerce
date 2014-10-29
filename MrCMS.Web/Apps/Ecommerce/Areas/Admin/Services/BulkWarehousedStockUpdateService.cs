using System.IO;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class BulkWarehousedStockUpdateService : IBulkWarehousedStockUpdateService
    {
        private readonly IBulkWarehousedStockUpdateValidationService _bulkWarehousedStockUpdateValidationService;
        private readonly IPerformWarehousedStockBulkUpdate _performWarehousedStockBulkUpdate;

        public BulkWarehousedStockUpdateService(
            IBulkWarehousedStockUpdateValidationService bulkWarehousedStockUpdateValidationService, IPerformWarehousedStockBulkUpdate performWarehousedStockBulkUpdate)
        {
            _bulkWarehousedStockUpdateValidationService = bulkWarehousedStockUpdateValidationService;
            _performWarehousedStockBulkUpdate = performWarehousedStockBulkUpdate;
        }

        public BulkStockUpdateResult Update(Stream file)
        {
            GetWarehouseStockFromFileResult result = _bulkWarehousedStockUpdateValidationService.ValidateFile(file);
            if (!result.IsSuccess)
                return BulkStockUpdateResult.Failure(result.Messages);
            var updateResult = _performWarehousedStockBulkUpdate.Update(result.DTOs);
            return updateResult.IsSuccess
                ? BulkStockUpdateResult.Success(updateResult.Messages)
                : BulkStockUpdateResult.Failure(updateResult.Messages);
        }
    }
}