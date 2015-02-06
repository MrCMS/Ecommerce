using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductSpecificationsService : IImportProductSpecificationsService
    {
        private readonly ISession _session;

        public ImportProductSpecificationsService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<ProductSpecificationValue> ImportSpecifications(
            ProductImportDataTransferObject dataTransferObject, Product product)
        {
            List<KeyValuePair<string, string>> specificationsToAdd =
                dataTransferObject.Specifications.Where(
                    s =>
                        !product.SpecificationValues.Select(value => value.SpecificationName)
                            .Contains(s.Key, StringComparer.InvariantCultureIgnoreCase)).ToList();
            List<ProductSpecificationValue> specificationsToRemove =
                product.SpecificationValues.Where(
                    value =>
                        !dataTransferObject.Specifications.Keys.Contains(value.SpecificationName,
                            StringComparer.InvariantCultureIgnoreCase))
                    .ToList();
            List<ProductSpecificationValue> specificationsToUpdate =
                product.SpecificationValues.Where(value => !specificationsToRemove.Contains(value)).ToList();
            foreach (var item in specificationsToAdd)
            {
                ProductSpecificationAttribute attribute =
                    _session.QueryOver<ProductSpecificationAttribute>()
                        .Where(
                            specificationAttribute =>
                                specificationAttribute.Name.IsInsensitiveLike(item.Key, MatchMode.Exact))
                        .Take(1)
                        .SingleOrDefault();
                if (attribute == null)
                {
                    attribute = new ProductSpecificationAttribute {Name = item.Key};
                    _session.Transact(session => session.Save(attribute));
                }

                SetValue(product, attribute, item.Value);
            }

            foreach (ProductSpecificationValue value in specificationsToRemove)
            {
                RemoveValue(product, value);
            }
            foreach (ProductSpecificationValue value in specificationsToUpdate)
            {
                ProductSpecificationAttribute attribute =
                    value.ProductSpecificationAttributeOption.ProductSpecificationAttribute;
                RemoveValue(product, value);

                SetValue(product, attribute, dataTransferObject.Specifications[value.SpecificationName]);
            }

            return dataTransferObject.Specifications.Any() ? product.SpecificationValues : null;
        }

        private void RemoveValue(Product product, ProductSpecificationValue value)
        {
            product.SpecificationValues.Remove(value);
            _session.Delete(value);
        }

        private static void SetValue(Product product, ProductSpecificationAttribute attribute, string value)
        {
            ProductSpecificationAttributeOption option = attribute.Options.FirstOrDefault(o => o.Name == value);
            if (option == null)
            {
                option = new ProductSpecificationAttributeOption
                {
                    Name = value,
                    ProductSpecificationAttribute = attribute
                };
                attribute.Options.Add(option);
            }
            product.SpecificationValues.Add(new ProductSpecificationValue
            {
                ProductSpecificationAttributeOption = option,
                Product = product
            });
        }
    }
}