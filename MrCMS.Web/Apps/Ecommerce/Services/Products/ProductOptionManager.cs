using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;
using MrCMS.Models;
using System;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductOptionManager : IProductOptionManager
    {
        private readonly ISession _session;
        private readonly IProductSearchService _productSearchService;

        public ProductOptionManager(ISession session, IProductSearchService productSearchService)
        {
            _session = session;
            _productSearchService = productSearchService;
        }

        public IList<ProductSpecificationAttribute> ListSpecificationAttributes()
        {
            return _session.QueryOver<ProductSpecificationAttribute>().Cacheable().List().OrderBy(x => x.DisplayOrder).ToList();
        }
        public ProductSpecificationAttribute GetSpecificationAttribute(int id)
        {
            return _session.QueryOver<ProductSpecificationAttribute>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
        public ProductSpecificationAttribute GetSpecificationAttributeByName(string name)
        {
            return _session.QueryOver<ProductSpecificationAttribute>()
                            .Where(
                                specificationOption =>
                                specificationOption.Name.IsInsensitiveLike(name, MatchMode.Exact)).SingleOrDefault();
        }
        public void AddSpecificationAttribute(ProductSpecificationAttribute option)
        {
            if (option == null || string.IsNullOrWhiteSpace(option.Name))
                return;
            _session.Transact(session => session.Save(option));
        }
        public void UpdateSpecificationAttribute(ProductSpecificationAttribute option)
        {
            _session.Transact(session => session.Update(option));
        }
        public void DeleteSpecificationAttribute(ProductSpecificationAttribute option)
        {
            _session.Transact(session => session.Delete(option));
        }
        public bool AnyExistingSpecificationAttributesWithName(string name)
        {
            return _session.QueryOver<ProductSpecificationAttribute>()
                           .Where(
                               specificationOption =>
                               specificationOption.Name.IsInsensitiveLike(name, MatchMode.Exact))
                           .RowCount() > 0;
        }
        public bool AnyExistingSpecificationAttributeOptionsWithName(string name, int id)
        {
            return _session.QueryOver<ProductSpecificationAttributeOption>()
                           .Where(
                               specificationOption =>
                               specificationOption.Name.IsInsensitiveLike(name, MatchMode.Exact) && specificationOption.ProductSpecificationAttribute.Id == id)
                           .RowCount() > 0;
        }
        public void UpdateSpecificationAttributeDisplayOrder(IList<SortItem> options)
        {
            _session.Transact(session => options.ForEach(item =>
            {
                var formItem = session.Get<ProductSpecificationAttribute>(item.Id);
                formItem.DisplayOrder = item.Order;
                session.Update(formItem);
            }));
        }

        public IList<ProductSpecificationAttributeOption> ListSpecificationAttributeOptions(int id)
        {
            return _session.QueryOver<ProductSpecificationAttributeOption>().Where(x => x.ProductSpecificationAttribute.Id == id).Cacheable().List();
        }
        public void AddSpecificationAttributeOption(ProductSpecificationAttributeOption option)
        {
            if (option == null || string.IsNullOrWhiteSpace(option.Name))
                return;
            option.ProductSpecificationAttribute.Options.Add(option);
            _session.Transact(session => session.Save(option));
        }
        public void UpdateSpecificationAttributeOption(ProductSpecificationAttributeOption option)
        {
            _session.Transact(session => session.Update(option));
        }
        public void UpdateSpecificationAttributeOptionDisplayOrder(IList<SortItem> options)
        {
            _session.Transact(session => options.ForEach(item =>
            {
                var formItem = session.Get<ProductSpecificationAttributeOption>(item.Id);
                formItem.DisplayOrder = item.Order;
                session.Update(formItem);
            }));
        }
        public void DeleteSpecificationAttributeOption(ProductSpecificationAttributeOption option)
        {
            _session.Transact(session => session.Delete(option));
        }

        public void SetSpecificationValue(Product product, ProductSpecificationAttribute productSpecificationAttribute, string value)
        {
            ProductSpecificationValue valueAlias = null;
            ProductSpecificationAttributeOption optionAlias = null;
            var specificationValue = _session.QueryOver(() => valueAlias)
                                 .JoinAlias(() => valueAlias.ProductSpecificationAttributeOption, () => optionAlias)
                                 .Where(
                                     () => valueAlias.Product == product &&
                                           optionAlias.ProductSpecificationAttribute == productSpecificationAttribute
                ).Cacheable().SingleOrDefault();

            var option =
                productSpecificationAttribute.Options.FirstOrDefault(
                    attributeOption =>
                    string.Equals(attributeOption.Name, value, StringComparison.InvariantCultureIgnoreCase));
            if (option == null)
            {
                option = new ProductSpecificationAttributeOption
                             {
                                 ProductSpecificationAttribute = productSpecificationAttribute,
                                 Name = value
                             };
                _session.Save(option);
            }

            if (specificationValue != null)
            {
                specificationValue.ProductSpecificationAttributeOption = option;
                _session.Transact(session => session.Update(specificationValue));
            }
            else
            {
                var productSpecificationValue = new ProductSpecificationValue
                {
                    Product = product,
                    ProductSpecificationAttributeOption = option
                };
                product.SpecificationValues.Add(productSpecificationValue);
                _session.Transact(session => session.SaveOrUpdate(product));
            }
        }
        public ProductSpecificationValue GetSpecificationValue(int id)
        {
            return _session.QueryOver<ProductSpecificationValue>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
        public void DeleteSpecificationValue(ProductSpecificationValue specification)
        {
            _session.Transact(session => session.Delete(specification));
        }
        public void UpdateSpecificationValueDisplayOrder(IList<SortItem> options)
        {
            _session.Transact(session => options.ForEach(item =>
            {
                var formItem = session.Get<ProductSpecificationValue>(item.Id);
                formItem.DisplayOrder = item.Order;
                session.Update(formItem);
            }));
        }

        public IList<ProductOption> GetAllAttributeOptions()
        {
            return _session.QueryOver<ProductOption>().Cacheable().List();
        }
        public IList<ProductOption> ListAttributeOptions()
        {
            return _session.QueryOver<ProductOption>().Cacheable().List();
        }
        public ProductOption GetAttributeOption(int id)
        {
            return _session.QueryOver<ProductOption>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
        public ProductOption GetAttributeOptionByName(string name)
        {
            return _session.QueryOver<ProductOption>()
                           .Where(
                               option =>
                               option.Name.IsInsensitiveLike(name, MatchMode.Exact)).SingleOrDefault();
        }
        public void AddAttributeOption(ProductOption productOption)
        {
            if (string.IsNullOrWhiteSpace(productOption.Name))
                return;
            if (!AnyExistingAttributeOptionsWithName(productOption))
                _session.Transact(session => session.Save(productOption));
        }
        public void UpdateAttributeOption(ProductOption option)
        {
            if (option == null || string.IsNullOrWhiteSpace(option.Name))
                return;
            if (AnyExistingAttributeOptionsWithName(option))
                return;
            _session.Transact(session => session.Update(option));
        }
        public void UpdateAttributeOption(string name, int id, Product product)
        {
            ProductOption optionById = GetAttributeOption(id);
            ProductOption optionByName = GetAttributeOptionByName(name);


            //if (id != 0 && option1 != null && option2 != null)
            //    return;
            //else if (id != 0 && option1 != null && option2 == null)
            //{
            //    option1.Name = name;
            //    _session.Transact(session => session.Update(option1));
            //}
            //else if (option2 != null)
            //{
            //    if (option2.Products.All(x => x.Id != product.Id))
            //    {
            //        option2.Products.Add(product);
            //    }
            //    _session.Transact(session => session.Update(option2));
            //    for (int i = 0; i < product.Variants.Count; i++)
            //    {
            //        if (product.Variants[i].OptionValues.All(x => x.ProductOption.Id != option2.Id))
            //        {
            //            product.Variants[i].OptionValues.Add(new ProductOptionValue
            //                {
            //                    ProductVariant = product.Variants[i],
            //                    ProductOption = option2,
            //                    Value = String.Empty
            //                });
            //            _session.Transact(session => session.Update(product.Variants[i]));
            //        }
            //    }
            //}
            //else
            //{
            //    ProductOption option = new ProductOption();
            //    option.Name = name;
            //    option.Products.Add(product);
            //    option.DisplayOrder = 0;
            //    _session.Transact(session => session.Save(option));

            //    for (int i = 0; i < product.Variants.Count; i++)
            //    {
            //        product.Variants[i].OptionValues.Add(new ProductOptionValue
            //            {
            //                ProductVariant = product.Variants[i],
            //                ProductOption = option,
            //                Value = String.Empty
            //            });
            //        _session.Transact(session => session.Update(product.Variants[i]));
            //    }
            //}

            //_session.Evict(typeof(ProductOptionValue));
            //_session.Evict(typeof(ProductOption));
            //_session.Evict(typeof(ProductVariant));
            //_session.Evict(typeof(Product));
        }
        public void UpdateAttributeOptionDisplayOrder(Product product, IList<SortItem> options)
        {
            if (product != null && options != null && options.Count > 0)
            {
                _session.Transact(session =>
                    {
                        options.ForEach(item =>
                            {
                                var option = session.Get<ProductOption>(item.Id);
                                
                                if (option == null)
                                    return;
                                
                                product.Options.Remove(option);
                                product.Options.Insert(item.Order, option);
                            });
                        session.Update(product);
                    });
            }
        }
        public void DeleteAttributeOption(ProductOption option)
        {
            _session.Transact(session => session.Delete(option));
        }
        public void SetAttributeValue(ProductVariant productVariant, string attributeName, string value)
        {
            var specificationOption = _session.QueryOver<ProductOption>().Where(option => option.Name == attributeName).Take(1).SingleOrDefault();
            if (specificationOption == null)
                return;
            var values =
                _session.QueryOver<ProductOptionValue>()
                        .Where(
                            specificationValue => specificationValue.ProductOption == specificationOption &&
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
                _session.Transact(session => session.Save(new ProductOptionValue
                {
                    ProductVariant = productVariant,
                    ProductOption = specificationOption,
                    Value = value
                }));
            }
        }
        public void DeleteProductAttributeValue(ProductOptionValue value)
        {
            _session.Transact(session => session.Delete(value));
        }
        public bool AnyExistingAttributeOptionsWithName(ProductOption option)
        {
            return _session.QueryOver<ProductOption>()
                           .Where(
                               specificationOption =>
                               specificationOption.Name.IsInsensitiveLike(option.Name, MatchMode.Exact))
                           .RowCount() > 0;
        }
        public bool AnyExistingAttributeOptionsWithName(string name, int id)
        {
            return _session.QueryOver<ProductOption>()
                           .Where(
                               option =>
                               option.Name.IsInsensitiveLike(name, MatchMode.Exact) && option.Id == id)
                           .RowCount() > 0;
        }
        public bool AnyExistingAttributeOptionsWithName(string name)
        {
            return _session.QueryOver<ProductOption>()
                           .Where(
                               option =>
                               option.Name.IsInsensitiveLike(name, MatchMode.Exact))
                           .RowCount() > 0;
        }

        public List<ProductOptionModel> GetSearchAttributeOptions(ProductSearchQuery query)
        {
            var values = _productSearchService.GetOptions(query);
            var productAttributeValues = _session.QueryOver<ProductOptionValue>().Fetch(value => value.ProductOption).Eager.Where(value => value.Id.IsIn(values)).Cacheable().List();

            var productAttributeOptions = productAttributeValues.Select(value => value.ProductOption).Distinct().ToList();

            return productAttributeOptions.Select(option => new ProductOptionModel
                                                                {
                                                                    Name = option.Name,
                                                                    Id = option.Id,
                                                                    Values =
                                                                        productAttributeValues.Where(
                                                                            value =>
                                                                            value.ProductOption == option).OrderBy(x => x.DisplayOrder)
                                                                                              .Distinct()
                                                                                              .Select(
                                                                                                  value =>
                                                                                                  new ProductValueModel
                                                                                                      {
                                                                                                          Name = value.Value,
                                                                                                          Id = value.Id
                                                                                                      }).ToList()
                                                                }).ToList();
        }

        public List<ProductOptionModel> GetSearchSpecificationAttributes(ProductSearchQuery query)
        {
            var values = _productSearchService.GetSpecifications(query);
            var productSpecificationAttributeOptions =
                _session.QueryOver<ProductSpecificationAttributeOption>()
                        .Fetch(value => value.ProductSpecificationAttribute)
                        .Eager.Where(option => option.Id.IsIn(values))
                        .Cacheable()
                        .List();
            var productSpecificationAttributes = productSpecificationAttributeOptions.Select(value => value.ProductSpecificationAttribute).Distinct().ToList();

            return productSpecificationAttributes.Select(attribute => new ProductOptionModel
                                                                          {
                                                                              Name = attribute.Name,
                                                                              Id = attribute.Id,
                                                                              Values =
                                                                                  productSpecificationAttributeOptions
                                                                                  .Where(
                                                                                      value =>
                                                                                      value
                                                                                          .ProductSpecificationAttribute ==
                                                                                      attribute).OrderBy(x => x.DisplayOrder)
                                                                                  .Distinct()
                                                                                  .Select(
                                                                                      value =>
                                                                                      new ProductValueModel
                                                                                          {
                                                                                              Name = value.Name,
                                                                                              Id = value.Id
                                                                                          }).ToList()
                                                                          }).ToList();
        }
    }
}