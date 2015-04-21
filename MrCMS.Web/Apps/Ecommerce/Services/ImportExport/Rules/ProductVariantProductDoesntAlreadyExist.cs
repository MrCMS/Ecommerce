using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantProductDoesntAlreadyExist : IProductImportValidationRule
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;

        public ProductVariantProductDoesntAlreadyExist(IProductService productService, IProductVariantService productVariantService)
        {
            _productService = productService;
            _productVariantService = productVariantService;
        }

        public IEnumerable<string> GetErrors(ProductImportDataTransferObject product)
        {
            var errors = new List<string>();
            var mrCmsProduct = _productService.GetByUrl(product.UrlSegment);

            foreach (var pv in product.ProductVariants)
            {
                var mrCmsVariant = _productVariantService.GetProductVariantBySKU(pv.SKU);

                if (mrCmsVariant != null)
                {
                    if (mrCmsProduct == null || (mrCmsVariant.Product.Id != mrCmsProduct.Id))
                    {
                        errors.Add(string.Format("Product Variant with SKU {0} already exists on product with Url Segment {1}", pv.SKU, mrCmsVariant.Product.UrlSegment));
                    }
                }
            }

            return errors;
        }
    }
}