using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class ProductOptionManager : IProductOptionManager
    {
        private readonly ISession _session;

        public ProductOptionManager(ISession session)
        {
            _session = session;
        }

        public IList<ProductSpecificationOption> ListSpecificationOptions()
        {
            return _session.QueryOver<ProductSpecificationOption>().Cacheable().List();
        }

        public void AddSpecificationOption(ProductSpecificationOption option)
        {
            if (option == null || string.IsNullOrWhiteSpace(option.Name))
                return;
            if (AnyExistingOptionsWithName(option))
                return;
            _session.Transact(session => session.Save(option));
        }

        public void UpdateSpecificationOption(ProductSpecificationOption option)
        {
            if (AnyExistingOptionsWithName(option))
                return;
            _session.Transact(session => session.Update(option));
        }

        public void DeleteSpecificationOption(ProductSpecificationOption option)
        {
            _session.Transact(session => session.Delete(option));
        }

        private bool AnyExistingOptionsWithName(ProductSpecificationOption option)
        {
            return _session.QueryOver<ProductSpecificationOption>()
                           .Where(
                               specificationOption =>
                               specificationOption.Name.IsInsensitiveLike(option.Name, MatchMode.Exact))
                           .RowCount() > 0;
        }
        private bool AnyExistingOptionsWithName(ProductAttributeOption option)
        {
            return _session.QueryOver<ProductAttributeOption>()
                           .Where(
                               specificationOption =>
                               specificationOption.Name.IsInsensitiveLike(option.Name, MatchMode.Exact))
                           .RowCount() > 0;
        }

        public void AddAttributeOption(ProductAttributeOption productAttributeOption)
        {
            if (string.IsNullOrWhiteSpace(productAttributeOption.Name))
                return;
            if (!AnyExistingOptionsWithName(productAttributeOption))
                _session.Transact(session => session.Save(productAttributeOption));
        }

        public void UpdateAttributeOptionn(ProductAttributeOption option)
        {
            if (option == null || string.IsNullOrWhiteSpace(option.Name))
                return;
            if (AnyExistingOptionsWithName(option))
                return;
            _session.Transact(session => session.Update(option));
        }

        public IList<ProductAttributeOption> ListAttributeOptions()
        {
            return _session.QueryOver<ProductAttributeOption>().Cacheable().List();
        }

        public void DeleteAttributeOption(ProductAttributeOption option)
        {
            _session.Transact(session => session.Delete(option));
        }

        public void SetSpecificationValue(Product product, string optionName, string value)
        {
            var specificationOption = _session.QueryOver<ProductSpecificationOption>().Where(option => option.Name == optionName).Take(1).SingleOrDefault();
            if (specificationOption == null)
                return;
            var values = _session.QueryOver<ProductSpecificationValue>().Where(specificationValue => specificationValue.Option == specificationOption && specificationValue.Product == product).Cacheable().List();
            if (values.Any())
            {
                var specificationValue = values.First();
                specificationValue.Value = value;
                _session.Transact(session => session.Update(specificationValue));
            }
            else
            {
                _session.Transact(session => session.Save(new ProductSpecificationValue
                {
                    Product = product,
                    Option = specificationOption,
                    Value = value
                }));
            }
        }

        public void SetAttributeValue(ProductVariant productVariant, string attributeName, string value)
        {
            var specificationOption = _session.QueryOver<ProductAttributeOption>().Where(option => option.Name == attributeName).Take(1).SingleOrDefault();
            if (specificationOption == null)
                return;
            var values =
                _session.QueryOver<ProductAttributeValue>()
                        .Where(
                            specificationValue => specificationValue.Option == specificationOption &&
                                                  specificationValue.ProductVariant == productVariant)
                        .Cacheable()
                        .List();
            if (values.Any())
            {
                var specificationValue = values.First();
                specificationValue.Value = value;
                _session.Transact(session => session.Update(specificationValue));
            }
            else
            {
                _session.Transact(session => session.Save(new ProductAttributeValue
                {
                    ProductVariant = productVariant,
                    Option = specificationOption,
                    Value = value
                }));
            }
        }
    }
}