using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate;

namespace MrCMS.EcommerceApp.Tests.Services.BulkStockUpdate
{
    public class BulkStockUpdateValidationServiceTests : InMemoryDatabaseTest
    {
        private IBulkStockUpdateValidationService _bulkStockUpdateValidationService;

        public BulkStockUpdateValidationServiceTests()
        {
            _bulkStockUpdateValidationService = new BulkStockUpdateValidationService();
        }
    }
}