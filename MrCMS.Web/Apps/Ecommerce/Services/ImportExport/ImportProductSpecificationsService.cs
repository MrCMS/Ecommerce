using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductSpecificationsService : IImportProductSpecificationsService
    {
        private readonly IProductOptionManager _productOptionManager;
        private readonly IProductVariantService _productVariantService;

        public ImportProductSpecificationsService(IProductOptionManager productOptionManager, IProductVariantService productVariantService)
        {
            _productOptionManager = productOptionManager;
            _productVariantService = productVariantService;
        }

        public IEnumerable<ProductSpecificationValue> ImportSpecifications(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            product.SpecificationValues.Clear();
            foreach (var item in dataTransferObject.Specifications)
            {
                if (!_productOptionManager.AnyExistingSpecificationAttributesWithName(item.Key))
                    _productOptionManager.AddSpecificationAttribute(new ProductSpecificationAttribute { Name = item.Key });
                var option = _productOptionManager.GetSpecificationAttributeByName(item.Key);
                if (option.Options.All(x => x.Name != item.Value))
                {
                    option.Options.Add(new ProductSpecificationAttributeOption
                                           {
                                               ProductSpecificationAttribute = option,
                                               Name = item.Value
                                           });
                    //_productOptionManager.UpdateSpecificationAttribute(option);
                }
                var optionValue = option.Options.SingleOrDefault(x => x.Name == item.Value);
                if (!product.SpecificationValues.Any(x => optionValue != null && (x.ProductSpecificationAttributeOption.Id == optionValue.Id && x.Product.Id == product.Id)))
                    product.SpecificationValues.Add(new ProductSpecificationValue
                                                        {
                                                            ProductSpecificationAttributeOption = optionValue,
                                                            Product = product,
                                                        });
            }

            return dataTransferObject.Specifications.Any() ? product.SpecificationValues : null;
        }

        public void ImportVariantSpecifications(ProductVariantImportDataTransferObject item, Product product, ProductVariant productVariant)
        {
            productVariant.AttributeValues.Clear();
            foreach (var opt in item.Options)
            {
                if (_productOptionManager.GetAttributeOptionByName(opt.Key)==null)
                    _productOptionManager.AddAttributeOption(new ProductAttributeOption { Name = opt.Key });
                var option = _productOptionManager.GetAttributeOptionByName(opt.Key);
                if (!product.AttributeOptions.Any(x => x.Id == option.Id))
                {
                    product.AttributeOptions.Add(option);
                }

                if (productVariant.AttributeValues.All(x => x.ProductAttributeOption.Id != option.Id))
                {
                    productVariant.AttributeValues.Add(new ProductAttributeValue
                    {
                        ProductAttributeOption = option,
                        ProductVariant = productVariant,
                        Value = opt.Value
                    });
                }
                else
                {
                    var productAttributeValue =
                        productVariant.AttributeValues.SingleOrDefault(x => x.ProductAttributeOption.Id == option.Id);
                    if (productAttributeValue != null)
                    {
                       productVariant.AttributeValues.SingleOrDefault(x => x.ProductAttributeOption.Id == option.Id).Value = opt.Value;
                    }
                }
            }
        }
    }
}