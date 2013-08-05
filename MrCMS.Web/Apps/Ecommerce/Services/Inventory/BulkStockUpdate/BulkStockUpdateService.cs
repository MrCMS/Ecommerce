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
        private IList<ProductVariant> _allVariants;

        public BulkStockUpdateService(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
            _allVariants = new List<ProductVariant>();
        }

        public int BulkStockUpdateFromDTOs(IEnumerable<BulkStockUpdateDataTransferObject> items)
        {
            _allVariants = _productVariantService.GetAll();

            var noOfUpdatedItems = 0;
            foreach (var dataTransferObject in items)
            {
                BulkStockUpdate(dataTransferObject, ref noOfUpdatedItems);
            }

            return noOfUpdatedItems;
        }

        private ProductVariant BulkStockUpdate(BulkStockUpdateDataTransferObject itemDto, ref int noOfUpdatedItems)
        {
            if (_allVariants == null)
                _allVariants = new List<ProductVariant>();

            var item = _allVariants.SingleOrDefault(x => x.SKU == itemDto.SKU);

            if (item != null && item.StockRemaining.Value!=itemDto.StockRemaining.Value)
            {
                item.StockRemaining = itemDto.StockRemaining;
                _productVariantService.Update(item);
                noOfUpdatedItems++;
            }

            return item;
        }
    }
}