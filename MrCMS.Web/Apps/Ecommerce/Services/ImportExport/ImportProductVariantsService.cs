using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductVariantsService : IImportProductVariantsService
    {
        private readonly IImportSpecificationsService _importSpecificationsService;
        private readonly IProductVariantService _productVariantService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IProductOptionManager _productOptionManager;

        public ImportProductVariantsService(IImportSpecificationsService importSpecificationsService, IProductVariantService productVariantService, ITaxRateManager taxRateManager, IProductOptionManager productOptionManager)
        {
            _importSpecificationsService = importSpecificationsService;
            _productVariantService = productVariantService;
            _taxRateManager = taxRateManager;
            _productOptionManager = productOptionManager;
        }

        public IEnumerable<ProductVariant> ImportVariants(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            foreach (var item in dataTransferObject.ProductVariants)
            {
                var productVariant = _productVariantService.GetProductVariantBySKU(item.SKU) ?? new ProductVariant();

                productVariant.Name = item.Name;
                productVariant.SKU = item.SKU;
                productVariant.Barcode = item.Barcode;
                productVariant.BasePrice = item.Price;
                productVariant.PreviousPrice = item.PreviousPrice;
                productVariant.StockRemaining = item.Stock;
                productVariant.Weight = item.Weight.HasValue ? item.Weight.Value : 0;
                productVariant.TrackingPolicy = item.TrackingPolicy;
                productVariant.TaxRate = _taxRateManager.Get(item.TaxRate.HasValue ? item.TaxRate.Value : 0);
                productVariant.Product = product;

                for (var i = productVariant.AttributeValues.Count - 1; i >= 0; i--)
                {
                    var value = productVariant.AttributeValues[i];
                    productVariant.AttributeValues.Remove(value);
                    _productOptionManager.DeleteProductAttributeValue(value);
                }

                _productVariantService.Update(productVariant);

                productVariant = _productVariantService.GetProductVariantBySKU(item.SKU);

               _importSpecificationsService.ImportVariantSpecifications(item, product, productVariant);

                if (!product.Variants.Any(x => x.Id == productVariant.Id))
                    product.Variants.Add(productVariant);

                _productVariantService.Update(productVariant);
            }

            return dataTransferObject.ProductVariants.Any() ? product.Variants : null;
        }

    }
}