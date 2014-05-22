using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly IProductSearchService _productSearchService;
        private readonly ISession _session;
        private readonly IUniquePageService _uniquePageService;

        public CategoryService(ISession session, IProductSearchService productSearchService, IUniquePageService uniquePageService)
        {
            _session = session;
            _productSearchService = productSearchService;
            _uniquePageService = uniquePageService;
        }

        public IPagedList<Category> GetCategories(Product product, string query, int page = 1)
        {
            QueryOver<Category, Category> queryOver = QueryOver.Of<Category>();

            if (!string.IsNullOrWhiteSpace(query))
                queryOver = queryOver.Where(category => category.Name.IsInsensitiveLike(query, MatchMode.Anywhere));

            queryOver = queryOver.Where(category => !category.Id.IsIn(product.Categories.Select(c => c.Id).ToArray()));

            return _session.Paged(queryOver, page);
        }

        public CategorySearchModel GetCategoriesForSearch(ProductSearchQuery query)
        {
            List<int> availableCategories = _productSearchService.GetCategories(query);
            if (!query.CategoryId.HasValue)
                return GetRootCategoryModel(availableCategories);

            var category = _session.Get<Category>(query.CategoryId);
            List<Category> categories =
                _session.QueryOver<Category>()
                    .Where(cat => cat.Parent.Id == category.Id && cat.Id.IsIn(availableCategories))
                    .Cacheable()
                    .List().ToList();
            List<Category> hierarchy =
                category.ActivePages.OfType<Category>().Where(cat => availableCategories.Contains(cat.Id)).ToList();
            hierarchy.Reverse();
            return new CategorySearchModel
                   {
                       Children = categories,
                       Hierarchy = hierarchy
                   };
        }

        private List<Category> GetRootCategories()
        {
            var productSearch = _uniquePageService.GetUniquePage<ProductSearch>();
            return productSearch == null
                ? new List<Category>()
                : _session.QueryOver<Category>()
                    .Where(category => category.Parent.Id == productSearch.Id)
                    .Cacheable()
                    .List()
                    .ToList();
        }

        private CategorySearchModel GetRootCategoryModel(List<int> availableCategories)
        {
            List<Category> categories = GetRootCategories().Where(cat => availableCategories.Contains(cat.Id)).ToList();
            return new CategorySearchModel
                   {
                       Children = categories
                   };
        }
    }
}