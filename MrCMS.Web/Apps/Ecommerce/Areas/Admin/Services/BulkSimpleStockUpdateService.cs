using System.IO;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class BulkSimpleStockUpdateService : IBulkSimpleStockUpdateService
    {
        private readonly IBulkSimpleStockUpdateValidationService _bulkSimpleStockUpdateValidationService;
        private readonly IPerformSimpleBulkUpdate _performSimpleBulkUpdate;

        public BulkSimpleStockUpdateService(
            IBulkSimpleStockUpdateValidationService bulkSimpleStockUpdateValidationService,
            IPerformSimpleBulkUpdate performSimpleBulkUpdate)
        {
            _bulkSimpleStockUpdateValidationService = bulkSimpleStockUpdateValidationService;
            _performSimpleBulkUpdate = performSimpleBulkUpdate;
        }

        public BulkStockUpdateResult Update(Stream file)
        {
            GetProductVariantsFromFileResult result = _bulkSimpleStockUpdateValidationService.ValidateFile(file);
            if (!result.IsSuccess)
                return BulkStockUpdateResult.Failure(result.Messages);
            BulkUpdateResult updateResult = _performSimpleBulkUpdate.Update(result.DTOs);
            return updateResult.IsSuccess
                ? BulkStockUpdateResult.Success(updateResult.Messages)
                : BulkStockUpdateResult.Failure(updateResult.Messages);
        }
    }
}