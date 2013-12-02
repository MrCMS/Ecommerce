using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductSpecificationsService : IImportProductSpecificationsService
    {
        private readonly ISession _session;
        private HashSet<ProductSpecificationAttribute> _productSpecificationAttributes;

        public ImportProductSpecificationsService(ISession session)
        {
            _session = session;
        }

        public HashSet<ProductSpecificationAttribute> ProductSpecificationAttributes
        {
            get { return _productSpecificationAttributes; }
        }

        public IImportProductSpecificationsService Initialize()
        {
            _productSpecificationAttributes = new HashSet<ProductSpecificationAttribute>(_session.QueryOver<ProductSpecificationAttribute>().List());
            return this;
        }

        public IEnumerable<ProductSpecificationValue> ImportSpecifications(ProductImportDataTransferObject dataTransferObject, Product product)
        {
            product.SpecificationValues.Clear();
            foreach (var item in dataTransferObject.Specifications)
            {
                ProductSpecificationAttribute specificationAttribute;
                if (ProductSpecificationAttributes.FirstOrDefault(
                    attribute => attribute.Name.Equals(item.Key, StringComparison.OrdinalIgnoreCase)) != null)
                    specificationAttribute = ProductSpecificationAttributes.FirstOrDefault(
                        attribute => attribute.Name.Equals(item.Key, StringComparison.OrdinalIgnoreCase));
                else
                {
                    specificationAttribute = new ProductSpecificationAttribute {Name = item.Key};
                    _productSpecificationAttributes.Add(specificationAttribute);
                }
                if (specificationAttribute.Options.All(x => x.Name != item.Value))
                {
                    specificationAttribute.Options.Add(new ProductSpecificationAttributeOption
                                           {
                                               ProductSpecificationAttribute = specificationAttribute,
                                               Name = item.Value
                                           });
                }
                var optionValue = specificationAttribute.Options.SingleOrDefault(x => x.Name == item.Value);
                if (!product.SpecificationValues.Any(x => optionValue != null && (x.ProductSpecificationAttributeOption.Id == optionValue.Id && x.Product.Id == product.Id)))
                    product.SpecificationValues.Add(new ProductSpecificationValue
                                                        {
                                                            ProductSpecificationAttributeOption = optionValue,
                                                            Product = product,
                                                        });
            }

            return dataTransferObject.Specifications.Any() ? product.SpecificationValues : null;
        }

    }
}