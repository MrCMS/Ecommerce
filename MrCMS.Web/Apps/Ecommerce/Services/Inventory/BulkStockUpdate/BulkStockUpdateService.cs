using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate
{
    public class BulkStockUpdateService : IBulkStockUpdateService
    {
        private readonly IProductVariantService _productVariantService;

        public BulkStockUpdateService(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        public int BulkStockUpdateFromDTOs(IEnumerable<BulkStockUpdateDataTransferObject> items)
        {

            var noOfUpdatedItems = 0;
            foreach (var dataTransferObject in items)
            {
                BulkStockUpdate(dataTransferObject, ref noOfUpdatedItems);
            }

            return noOfUpdatedItems;
        }

        public ProductVariant BulkStockUpdate(BulkStockUpdateDataTransferObject itemDto, ref int noOfUpdatedItems)
        {
            var item = _productVariantService.GetProductVariantBySKU(itemDto.SKU);

            if (itemDto.StockRemaining != null && (item.StockRemaining != null && (item.StockRemaining.Value!=itemDto.StockRemaining.Value)))
            {
                item.StockRemaining = itemDto.StockRemaining;
                _productVariantService.Update(item);
                noOfUpdatedItems++;
                return item;
            }
            return new ProductVariant();
        }
    }
}