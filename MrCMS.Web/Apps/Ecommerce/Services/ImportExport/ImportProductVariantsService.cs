using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductVariantsService : IImportProductVariantsService
    {
        private readonly IImportProductVariantPriceBreaksService _importProductVariantPriceBreaksService;
        private readonly IImportProductSpecificationsService _importProductSpecificationsService;
        private readonly IProductVariantService _productVariantService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IDocumentService _documentService;

        public ImportProductVariantsService(IImportProductVariantPriceBreaksService importPriceBreaksService, IImportProductSpecificationsService importSpecificationsService, 
            IProductVariantService productVariantService, ITaxRateManager taxRateManager,
            IProductOptionManager productOptionManager, IDocumentService documentService)
        {
            _importProductSpecificationsService = importSpecificationsService;
            _productVariantService = productVariantService;
            _taxRateManager = taxRateManager;
            _productOptionManager = productOptionManager;
            _documentService = documentService;
            _importProductVariantPriceBreaksService = importPriceBreaksService;
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
                productVariant.TaxRate = (item.TaxRate.HasValue && item.TaxRate.Value!=0)?_taxRateManager.Get(item.TaxRate.Value):_taxRateManager.GetDefaultRate();
                productVariant.Product = product;

                product.Variants.Add(productVariant);

                //_productVariantService.Add(productVariant);
                //_documentService.SaveDocument(product);

                for (var i = productVariant.AttributeValues.Count - 1; i >= 0; i--)
                {
                    var value = productVariant.AttributeValues[i];
                    productVariant.AttributeValues.Remove(value);
                    _productOptionManager.DeleteProductAttributeValue(value);
                }

                //productVariant = _productVariantService.GetProductVariantBySKU(item.SKU);

                //Price Breaks
                _importProductVariantPriceBreaksService.ImportVariantPriceBreaks(item,productVariant);

                //Specifications
               _importProductSpecificationsService.ImportVariantSpecifications(item, product, productVariant);
            }

            return dataTransferObject.ProductVariants.Any() ? product.Variants : null;
        }
    }
}