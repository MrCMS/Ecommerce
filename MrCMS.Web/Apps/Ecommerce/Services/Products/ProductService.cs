using System.Linq;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;
using System.Collections.Generic;
using System.Web.Mvc;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;
        private readonly SiteSettings _ecommerceSettings;

        public ProductService(ISession session, IDocumentService documentService, SiteSettings ecommerceSettings)
        {
            _session = session;
            _documentService = documentService;
            _ecommerceSettings = ecommerceSettings;
        }

        public ProductPagedList Search(string queryTerm = null, int page = 1)
        {
            IPagedList<Product> pagedList;
            var pageSize = _ecommerceSettings.DefaultPageSize > 0 ? _ecommerceSettings.DefaultPageSize : 10;
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
                                    .Paged(page, pageSize);
            }
            else
            {
                pagedList = _session.Paged(QueryOver.Of<Product>(), page, pageSize);
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

        public IPagedList<Product> Search(Product product, string query, int page = 1, int pageSize = 10)
        {
            var queryOver = QueryOver.Of<Product>();

            if (!string.IsNullOrWhiteSpace(query))
                queryOver = queryOver.Where(item => item.Name.IsInsensitiveLike(query, MatchMode.Anywhere));

            queryOver = queryOver.Where(item => !item.Id.IsIn(product.RelatedProducts.Select(c => c.Id).ToArray()) && item.Id != product.Id);

            return _session.Paged(queryOver, page, pageSize);
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

        public void AddRelatedProduct(Product product, int relatedProductId)
        {
            var relatedProduct = _documentService.GetDocument<Product>(relatedProductId);

            if (product.RelatedProducts.Any(x => x.Id == relatedProductId)) return;

            product.RelatedProducts.Add(relatedProduct);
            _session.Transact(session => session.SaveOrUpdate(product));
        }

        public void RemoveRelatedProduct(Product product, int relatedProductId)
        {
            var relatedProduct = _documentService.GetDocument<Product>(relatedProductId);
            product.RelatedProducts.Remove(relatedProduct);
            relatedProduct.RelatedProducts.Remove(product);
            _session.Transact(session =>
            {
                session.SaveOrUpdate(product);
                session.SaveOrUpdate(relatedProduct);
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

        public Product Update(Product product)
        {
            _session.Transact(session => session.Update(product));
            return product;
        }

        public void SetVariantOrders(Product product, List<SortItem> items)
        {
            _session.Transact(session =>
                                  {
                                      items.ForEach(item =>
                                                        {
                                                            var variant = session.Get<ProductVariant>(item.Id);
                                                            if (variant != null)
                                                            {
                                                                product.Variants.Remove(variant);
                                                                product.Variants.Insert(item.Order, variant);
                                                            }
                                                        });
                                      session.Update(product);
                                  });
        }

        public IList<Product> GetNewIn(int numberOfItems = 10)
        {
            var ids =
                _session.QueryOver<Product>()
                        .Where(x => x.PublishOn <= CurrentRequestData.Now)
                        .Select(product => product.Id)
                        .OrderBy(x => x.CreatedOn)
                        .Desc.Take(numberOfItems)
                        .List<int>();
            return ids.Select(i => _session.Get<Product>(i)).ToList();
        }
    }
}