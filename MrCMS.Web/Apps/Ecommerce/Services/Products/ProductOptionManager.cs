using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductOptionManager : IProductOptionManager
    {
        private readonly IProductSearchService _productSearchService;
        private readonly IUniquePageService _uniquePageService;
        private readonly ISession _session;

        public ProductOptionManager(ISession session, IProductSearchService productSearchService,IUniquePageService uniquePageService)
        {
            _session = session;
            _productSearchService = productSearchService;
            _uniquePageService = uniquePageService;
        }

        public IList<ProductSpecificationAttribute> ListSpecificationAttributes()
        {
            return
                _session.QueryOver<ProductSpecificationAttribute>()
                        .Cacheable()
                        .List()
                        .OrderBy(x => x.DisplayOrder)
                        .ToList();
        }

        public ProductSpecificationAttribute GetSpecificationAttribute(int id)
        {
            return _session.Get<ProductSpecificationAttribute>(id);
        }

        public ProductSpecificationAttribute GetSpecificationAttributeByName(string name)
        {
            return
                _session.QueryOver<ProductSpecificationAttribute>()
                        .Where(specificationOption => specificationOption.Name == name)
                        .SingleOrDefault();
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

        public bool AnyExistingSpecificationAttributesWithName(UniqueAttributeNameModel model)
        {
            return
                _session.QueryOver<ProductSpecificationAttribute>()
                        .Where(specificationOption => specificationOption.Name == model.Name && specificationOption.Id != model.Id)
                        .Any();
        }

        public bool AnyExistingSpecificationAttributeOptionsWithName(string name, int id)
        {
            return
                _session.QueryOver<ProductSpecificationAttributeOption>()
                        .Where(
                            specificationOption =>
                            specificationOption.Name == name &&
                            specificationOption.ProductSpecificationAttribute.Id == id)
                        .RowCount() > 0;
        }

        public void UpdateSpecificationAttributeDisplayOrder(IList<SortItem> options)
        {
            _session.Transact(session => options.ForEach(item =>
                                                             {
                                                                 var formItem =
                                                                     session.Get<ProductSpecificationAttribute>(item.Id);
                                                                 formItem.DisplayOrder = item.Order;
                                                                 session.Update(formItem);
                                                             }));
        }

        public IList<ProductSpecificationAttributeOption> ListSpecificationAttributeOptions(int id)
        {
            return
                _session.QueryOver<ProductSpecificationAttributeOption>()
                        .Where(x => x.ProductSpecificationAttribute.Id == id)
                        .Cacheable()
                        .List();
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
                                                                 var formItem =
                                                                     session.Get<ProductSpecificationAttributeOption>(
                                                                         item.Id);
                                                                 formItem.DisplayOrder = item.Order;
                                                                 session.Update(formItem);
                                                             }));
        }

        public void DeleteSpecificationAttributeOption(ProductSpecificationAttributeOption option)
        {
            _session.Transact(session => session.Delete(option));
        }

        public void SetSpecificationValue(Product product, ProductSpecificationAttribute productSpecificationAttribute,
                                          string value)
        {
            ProductSpecificationValue valueAlias = null;
            ProductSpecificationAttributeOption optionAlias = null;
            ProductSpecificationValue specificationValue = _session.QueryOver(() => valueAlias)
                                                                   .JoinAlias(
                                                                       () =>
                                                                       valueAlias.ProductSpecificationAttributeOption,
                                                                       () => optionAlias)
                                                                   .Where(
                                                                       () => valueAlias.Product == product &&
                                                                             optionAlias.ProductSpecificationAttribute ==
                                                                             productSpecificationAttribute
                ).Cacheable().SingleOrDefault();

            ProductSpecificationAttributeOption option =
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
            return _session.Get<ProductSpecificationValue>(id);
        }

        public void DeleteSpecificationValue(ProductSpecificationValue specification)
        {
            specification.Product.SpecificationValues.Remove(specification);
            _session.Transact(session => session.Delete(specification));
        }

        public void UpdateSpecificationValueDisplayOrder(IList<SortItem> options)
        {
            _session.Transact(session => options.ForEach(item =>
                                                             {
                                                                 var formItem =
                                                                     session.Get<ProductSpecificationValue>(item.Id);
                                                                 formItem.DisplayOrder = item.Order;
                                                                 session.Update(formItem);
                                                             }));
        }

        public ProductOption GetAttributeOptionByName(string name)
        {
            return _session.QueryOver<ProductOption>().Where(option => option.Name == name).SingleOrDefault();
        }

        public void AddAttributeOption(ProductOption productOption)
        {
            if (string.IsNullOrWhiteSpace(productOption.Name))
                return;
            if (!AnyExistingAttributeOptionsWithName(productOption))
                _session.Transact(session => session.Save(productOption));
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

        public void DeleteProductAttributeValue(ProductOptionValue value)
        {
            _session.Transact(session => session.Delete(value));
        }

        public List<ProductOptionModel> GetSearchAttributeOptions(ProductSearchQuery query)
        {
            List<int> values = _productSearchService.GetOptions(query);
            IList<ProductOptionValue> productAttributeValues =
                _session.QueryOver<ProductOptionValue>()
                        .Fetch(value => value.ProductOption)
                        .Eager.Where(value => value.Id.IsIn(values))
                        .Cacheable()
                        .List();

            List<ProductOption> productAttributeOptions =
                productAttributeValues.Select(value => value.ProductOption).Distinct().ToList();

            return productAttributeOptions.Select(option => new ProductOptionModel
                                                                {
                                                                    Name = option.Name,
                                                                    Id = option.Id,
                                                                    Values =
                                                                        productAttributeValues.Where(
                                                                            value =>
                                                                            value.ProductOption == option)
                                                                                              .OrderBy(
                                                                                                  x => x.DisplayOrder)
                                                                                              .Distinct()
                                                                                              .Select(
                                                                                                  value =>
                                                                                                  new ProductValueModel
                                                                                                      {
                                                                                                          Name =
                                                                                                              value
                                                                                                              .Value,
                                                                                                          Id = value.Id
                                                                                                      }).ToList()
                                                                }).ToList();
        }

        public List<ProductOptionModel> GetSearchSpecificationAttributes(ProductSearchQuery query)
        {
            List<int> values = _productSearchService.GetSpecifications(query);
            ProductSpecificationAttribute attributeAlias = null;
            IList<ProductSpecificationAttributeOption> productSpecificationAttributeOptions =
                _session.QueryOver<ProductSpecificationAttributeOption>()
                        .JoinAlias(option => option.ProductSpecificationAttribute, () => attributeAlias)
                        .Where(option => option.Id.IsIn(values) && !attributeAlias.HideInSearch)
                        .Fetch(option => option.ProductSpecificationAttribute).Eager
                        .Cacheable()
                        .List();
            List<ProductSpecificationAttribute> productSpecificationAttributes =
                productSpecificationAttributeOptions.Select(value => value.ProductSpecificationAttribute)
                                                    .OrderBy(x => x.DisplayOrder)
                                                    .Distinct()
                                                    .ToList();

            RemoveHiddenSearchSpecifications(query, productSpecificationAttributes);

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
                                                                                      attribute)
                                                                                  .OrderBy(x => x.DisplayOrder)
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

        private void RemoveHiddenSearchSpecifications(ProductSearchQuery query, List<ProductSpecificationAttribute> productSpecificationAttributes)
        {
            var category = query.CategoryId.HasValue
                ? (EcommerceSearchablePage)_session.Get<Category>(query.CategoryId.Value)
                : _uniquePageService.GetUniquePage<ProductSearch>();
            if (category != null)
            {
                productSpecificationAttributes.RemoveAll(
                    attribute => category.HiddenSearchSpecifications.Contains(attribute));
            }
        }

        public IList<ProductOption> ListAttributeOptions()
        {
            return _session.QueryOver<ProductOption>().Cacheable().List();
        }

        public void UpdateAttributeOption(ProductOption option)
        {
            if (option == null || string.IsNullOrWhiteSpace(option.Name))
                return;
            if (AnyExistingAttributeOptionsWithName(option))
                return;
            _session.Transact(session => session.Update(option));
        }

        public void DeleteAttributeOption(ProductOption option)
        {
            _session.Transact(session => session.Delete(option));
        }

        public void SetAttributeValue(ProductVariant productVariant, string attributeName, string value)
        {
            ProductOption specificationOption =
                _session.QueryOver<ProductOption>()
                        .Where(option => option.Name == attributeName)
                        .Take(1)
                        .SingleOrDefault();
            if (specificationOption == null)
                return;
            IList<ProductOptionValue> values =
                _session.QueryOver<ProductOptionValue>()
                        .Where(
                            specificationValue => specificationValue.ProductOption == specificationOption &&
                                                  specificationValue.ProductVariant == productVariant)
                        .Cacheable()
                        .List();
            if (values.Any())
            {
                ProductOptionValue specificationValue = values.First();
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

        public bool AnyExistingAttributeOptionsWithName(ProductOption option)
        {
            return _session.QueryOver<ProductOption>()
                           .Where(
                               specificationOption =>
                               specificationOption.Name.IsInsensitiveLike(option.Name, MatchMode.Exact))
                           .RowCount() > 0;
        }
    }
}