using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using MrCMS.Helpers;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Web.Mvc;

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
                pagedList =
                    _session.Paged(
                        QueryOver.Of<Product>()
                                 .Where(product => product.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)), page,
                        10);
            }
            else
            {
                pagedList = _session.Paged(QueryOver.Of<Product>(), page, 10);
            }

            var productContainer = _documentService.GetUniquePage<ProductContainer>();
            var productContainerId = productContainer == null ? (int?)null : productContainer.Id;
            return new ProductPagedList(pagedList, productContainerId);
        }

        public void MakeMultiVariant(Product product, string option1, string option2, string option3)
        {
            product.HasMultiVariants = true;

            if (!string.IsNullOrWhiteSpace(option1))
            {
                var productAttributeOption =
                    _session.QueryOver<ProductAttributeOption>()
                            .Where(option => option.Name.IsInsensitiveLike(option1, MatchMode.Exact))
                            .Take(1)
                            .SingleOrDefault() ?? new ProductAttributeOption { Name = option1 };
                product.AttributeOptions.Add(productAttributeOption);
            }
            if (!string.IsNullOrWhiteSpace(option2))
            {
                var productAttributeOption =
                    _session.QueryOver<ProductAttributeOption>()
                            .Where(option => option.Name.IsInsensitiveLike(option2, MatchMode.Exact))
                            .Take(1)
                            .SingleOrDefault()
                    ?? new ProductAttributeOption { Name = option2 };
                product.AttributeOptions.Add(productAttributeOption);
            }
            if (!string.IsNullOrWhiteSpace(option3))
            {
                var productAttributeOption =
                    _session.QueryOver<ProductAttributeOption>()
                            .Where(option => option.Name.IsInsensitiveLike(option3, MatchMode.Exact))
                            .Take(1)
                            .SingleOrDefault()
                    ?? new ProductAttributeOption { Name = option3 };
                product.AttributeOptions.Add(productAttributeOption);
            }
            _session.Transact(session =>
                                  {
                                      session.SaveOrUpdate(product);
                                      product.AttributeOptions.ForEach(session.SaveOrUpdate);
                                  });
        }

        public void AddCategory(Product product, int categoryId)
        {
            var category = _documentService.GetDocument<Category>(categoryId);
            product.Categories.Add(category);
            category.Products.Add(product);
            _session.Transact(session => session.SaveOrUpdate(product));
        }

        public void RemoveCategory(Product product, int categoryId)
        {
            var category = _documentService.GetDocument<Category>(categoryId);
            product.Categories.Remove(category);
            category.Products.Remove(product);
            _session.Transact(session => session.SaveOrUpdate(product));
        }

        public List<SelectListItem> GetOptions()
        {
            return _session.QueryOver<Product>().Cacheable().List().BuildSelectItemList(item => item.Name, item => item.Id.ToString(), emptyItemText: null);
        }

        public Product Get(int id)
        {
            return _session.QueryOver<Product>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
    }
}