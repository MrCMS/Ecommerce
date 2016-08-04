using System;
using System.Linq;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;
using System.Collections.Generic;
using System.Web.Mvc;
using NHibernate.Criterion;
using Brand = MrCMS.Web.Apps.Ecommerce.Pages.Brand;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;
        private readonly SiteSettings _ecommerceSettings;
        private readonly IUniquePageService _uniquePageService;

        public ProductService(ISession session, IDocumentService documentService, SiteSettings ecommerceSettings, IUniquePageService uniquePageService)
        {
            _session = session;
            _documentService = documentService;
            _ecommerceSettings = ecommerceSettings;
            _uniquePageService = uniquePageService;
        }

        public ProductVariantPagedList Search(string queryTerm = null, int page = 1)
        {
            IPagedList<ProductVariant> pagedList;

            var pageSize = _ecommerceSettings.DefaultPageSize > 0 ? _ecommerceSettings.DefaultPageSize : 10;
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                pagedList = _session.QueryOver<ProductVariant>()
                                    .Where(x => x.SKU == queryTerm || x.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere))
                                    .Paged(page, pageSize);
            }
            else
            {
                pagedList = _session.Paged(QueryOver.Of<ProductVariant>(), page, pageSize);
            }

            var productContainer = _uniquePageService.GetUniquePage<ProductContainer>();
            var productContainerId = productContainer == null ? (int?)null : productContainer.Id;
            return new ProductVariantPagedList(pagedList, productContainerId);
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

        public IPagedList<Product> RelatedProductsSearch(Product product, string query, int page = 1)
        {
            var queryOver = QueryOver.Of<Product>();

            if (!string.IsNullOrWhiteSpace(query))
                queryOver = queryOver.Where(item => item.Name.IsInsensitiveLike(query, MatchMode.Anywhere));

            queryOver = queryOver.Where(item => !item.Id.IsIn(product.RelatedProducts.Select(c => c.Id).ToArray()) && item.Id != product.Id);

            return _session.Paged(queryOver, page);
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

            if (product.RelatedProducts.Any(x => x.Id == relatedProductId))
                return;

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

        public List<SelectListItem> GetPublishStatusOptions()
        {
            return Enum.GetValues(typeof(PublishStatus))
                .Cast<PublishStatus>()
                .BuildSelectItemList(status => status.ToString(), emptyItem: null);
        }

        public IPagedList<Product> Search(ProductAdminSearchQuery query)
        {
            Product productAlias = null;

            var queryOver = _session.QueryOver(() => productAlias);

            switch (query.PublishStatus)
            {
                case PublishStatus.Unpublished:
                    queryOver = queryOver.Where(product => product.PublishOn == null || product.PublishOn > CurrentRequestData.Now);
                    break;
                case PublishStatus.Published:
                    queryOver = queryOver.Where(product => product.PublishOn != null && product.PublishOn <= CurrentRequestData.Now);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(query.Brand))
            {
                Brand brandAlias = null;
                queryOver = queryOver.JoinAlias(product => product.BrandPage, () => brandAlias)
                    .Where(() => brandAlias.Name.IsInsensitiveLike(query.Brand, MatchMode.Anywhere));
            }

            if (!string.IsNullOrWhiteSpace(query.CategoryName))
            {
                Product categoryProductAlias = null;
                queryOver = queryOver.WithSubquery.WhereExists(QueryOver.Of<Category>()
                            .JoinAlias(category => category.Products, () => categoryProductAlias)
                            .Where(x => x.Name.IsInsensitiveLike(query.CategoryName, MatchMode.Anywhere)
                                    && categoryProductAlias.Id == productAlias.Id)
                            .Select(x => x.Id));
            }

            if (!string.IsNullOrWhiteSpace(query.SKU))
            {
                queryOver = queryOver.WithSubquery.WhereExists(QueryOver.Of<ProductVariant>()
                            .Where(x => x.Product.Id == productAlias.Id
                                && x.SKU.IsInsensitiveLike(query.SKU, MatchMode.Anywhere))
                            .Select(x => x.Id));
            }

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                queryOver = queryOver.Where(x => x.Name.IsInsensitiveLike(query.Name, MatchMode.Anywhere));
            }

            var min = query.PriceFrom ?? 0;
            var max = query.PriceTo ?? int.MaxValue;
            queryOver = queryOver.WithSubquery.WhereExists(QueryOver.Of<ProductVariant>()
                            .Where(x => x.Product.Id == productAlias.Id && x.BasePrice >= min && x.BasePrice <= max)
                            .Select(x => x.Id));

            return queryOver.OrderBy(product => product.Name).Asc.Paged(query.Page);
        }

        public IList<Product> GetNewIn(int numberOfItems = 10)
        {
            return _session.QueryOver<Product>()
                    .Where(x => x.PublishOn <= CurrentRequestData.Now)
                    .OrderBy(x => x.CreatedOn)
                    .Desc.Take(numberOfItems)
                    .List();
        }
    }
}