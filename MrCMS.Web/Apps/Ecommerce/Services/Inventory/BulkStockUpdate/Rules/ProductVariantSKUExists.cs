using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.Rules
{
    public class ProductVariantSKUExists : IBulkStockUpdateValidationRule
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantSKUExists(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService; 
        }

        public IEnumerable<string> GetErrors(BulkStockUpdateDataTransferObject productVariant)
        {
            if (_productVariantService.GetProductVariantBySKU(productVariant.SKU) == null)
                yield return string.Format("Product Variant with SKU: {0} is not present within the system.", productVariant.SKU);
        }
    }
}