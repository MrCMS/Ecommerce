using System;
using System.Collections;
using System.Linq;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using MrCMS.Helpers;
using System.Collections.Generic;
using System.Web.Mvc;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Linq;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;

        public ProductService(ISession session, IDocumentService documentService)
        {
            _session = session;
            _documentService = documentService;
        }

        public ProductPagedList Search(string queryTerm = null, int page = 1)
        {
            IPagedList<Product> pagedList;
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                Product productAlias = null;
                ProductVariant productVariantAlias = null;
                pagedList = _session.QueryOver(() => productAlias)
                                    .JoinAlias(() => productAlias.Variants, () => productVariantAlias, JoinType.LeftOuterJoin)
                                    .Where(
                                        () =>
                                        productVariantAlias.SKU == queryTerm ||
                                        productAlias.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere) ||
                                        productVariantAlias.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere))
                                    .Paged(page, 10);
            }
            else
            {
                pagedList = _session.Paged(QueryOver.Of<Product>(), page, 10);
            }

            var productContainer = _documentService.GetUniquePage<ProductSearch>();
            var productContainerId = productContainer == null ? (int?)null : productContainer.Id;
            return new ProductPagedList(pagedList, productContainerId);
        }

        public IList<Product> Search(string queryTerm)
        {
            return !string.IsNullOrWhiteSpace(queryTerm)
                       ? _session.QueryOver<Product>()
                                 .Where(product => product.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere))
                                 .Cacheable()
                                 .List()
                       : new List<Product>();
        }

        private IList<ProductAttributeValue> GetAttributeValues(MakeMultivariantModel model, ProductVariant productVariant)
        {
            var values = new List<ProductAttributeValue>();
            if (!string.IsNullOrWhiteSpace(model.Option1))
                values.Add(new ProductAttributeValue
                               {
                                   ProductAttributeOption =
                                       _session.QueryOver<ProductAttributeOption>()
                                               .Where(
                                                   option =>
                                                   option.Name.IsInsensitiveLike(model.Option1, MatchMode.Exact))
                                               .SingleOrDefault(),
                                   Value = model.Option1Value,
                                   ProductVariant = productVariant
                               });
            if (!string.IsNullOrWhiteSpace(model.Option2))
                values.Add(new ProductAttributeValue
                               {
                                   ProductAttributeOption =
                                       _session.QueryOver<ProductAttributeOption>()
                                               .Where(
                                                   option =>
                                                   option.Name.IsInsensitiveLike(model.Option2, MatchMode.Exact))
                                               .SingleOrDefault(),
                                   Value = model.Option2Value,
                                   ProductVariant = productVariant
                               });
            if (!string.IsNullOrWhiteSpace(model.Option3))
                values.Add(new ProductAttributeValue
                               {
                                   ProductAttributeOption =
                                       _session.QueryOver<ProductAttributeOption>()
                                               .Where(
                                                   option =>
                                                   option.Name.IsInsensitiveLike(model.Option3, MatchMode.Exact))
                                               .SingleOrDefault(),
                                   Value = model.Option3Value,
                                   ProductVariant = productVariant
                               });
            return values;
        }

        private void AddAttributeOption(string optionName, Product product)
        {
            var productAttributeOption =
                _session.QueryOver<ProductAttributeOption>()
                        .Where(option => option.Name.IsInsensitiveLike(optionName, MatchMode.Exact))
                        .Take(1)
                        .SingleOrDefault();
            if (productAttributeOption == null)
            {
                _session.Transact(session => session.Save(new ProductAttributeOption() { Name = optionName }));
                productAttributeOption =
                _session.QueryOver<ProductAttributeOption>()
                        .Where(option => option.Name.IsInsensitiveLike(optionName, MatchMode.Exact))
                        .Take(1)
                        .SingleOrDefault();
            }
            product.AttributeOptions.Add(productAttributeOption);
        }

        public void AddCategory(Product product, int categoryId)
        {
            var category = _documentService.GetDocument<Category>(categoryId);
            product.Categories.Add(category);
            category.Products.Add(product);
            _session.Transact(session =>
                                  {
                                      session.SaveOrUpdate(product);
                                      session.SaveOrUpdate(category);
                                  });
        }

        public void RemoveCategory(Product product, int categoryId)
        {
            var category = _documentService.GetDocument<Category>(categoryId);
            product.Categories.Remove(category);
            category.Products.Remove(product);
            _session.Transact(session =>
                                  {
                                      session.SaveOrUpdate(product);
                                      session.SaveOrUpdate(category);
                                  });
        }

        public List<SelectListItem> GetOptions()
        {
            return _session.QueryOver<Product>().Cacheable().List().BuildSelectItemList(item => item.Name, item => item.Id.ToString(), emptyItemText: null);
        }

        public Product Get(int id)
        {
            return _session.QueryOver<Product>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public Product GetByName(string name)
        {
            return _session.QueryOver<Product>()
                           .Where(
                               product =>
                               product.Name.IsInsensitiveLike(name, MatchMode.Exact))
                           .SingleOrDefault();
        }
        public Product GetByUrl(string url)
        {
            return _session.QueryOver<Product>()
                           .Where(
                               product =>
                               product.UrlSegment.IsInsensitiveLike(url, MatchMode.Exact))
                           .SingleOrDefault();
        }
        public IList<Product> GetAll()
        {
            return _session.QueryOver<Product>().Cacheable().List();
        }

        public void SetCategoryOrder(Product product, List<SortItem> items)
        {
            _session.Transact(session =>
                {
                    items.ForEach(item =>
                        {
                            var category = session.Get<Category>(item.Id);
                            if (category != null)
                            {
                                product.Categories.Remove(category);
                                product.Categories.Insert(item.Order, category);
                            }
                        });
                    session.Update(product);
                }
            );
        }
    }
}