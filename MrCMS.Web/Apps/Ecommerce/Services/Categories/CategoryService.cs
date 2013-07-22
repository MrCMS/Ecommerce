using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Apps.Ecommerce.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;
        private readonly CurrentSite _currentSite;

        public CategoryService(ISession session, CurrentSite currentSite, IDocumentService documentService)
        {
            _session = session;
            _currentSite = currentSite;
            _documentService = documentService;
        }

        public CategoryPagedList Search(string queryTerm = null, int page = 1)
        {
            IPagedList<Category> pagedList;
            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                pagedList =
                    _session.Paged(
                        QueryOver.Of<Category>()
                                 .Where(category => category.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)), page,
                        10);
            }
            else
            {
                pagedList = _session.Paged(QueryOver.Of<Category>(), page, 10);
            }

            var categoryContainer = _documentService.GetUniquePage<CategoryContainer>();
            var categoryContainerId = categoryContainer == null ? (int?)null : categoryContainer.Id;
            return new CategoryPagedList(pagedList, categoryContainerId);
        }

        public IEnumerable<AutoCompleteResult> Search(string query, List<int> ids)
        {
            return _session.QueryOver<Category>()
                           .Where(
                               category =>
                               !category.Id.IsIn(ids) && category.Name.IsInsensitiveLike(query, MatchMode.Anywhere))
                           .Take(5)
                           .Cacheable()
                           .List().Select(category => new AutoCompleteResult
                                                          {
                                                              id = category.Id,
                                                              label = category.NestedName,
                                                              value = category.NestedName
                                                          });
        }

        public IPagedList<Category> GetCategories(Product product, string query, int page)
        {
            var queryOver = QueryOver.Of<Category>();

            if (!string.IsNullOrWhiteSpace(query))
                queryOver = queryOver.Where(category => category.Name.IsInsensitiveLike(query, MatchMode.Anywhere));

            queryOver = queryOver.Where(category => !category.Id.IsIn(product.Categories.Select(c => c.Id).ToArray()));

            return _session.Paged(queryOver, page, 10);
        }
        public IList<Category> GetAll()
        {
            return _session.QueryOver<Category>().Cacheable().List();
        }
        public Category Get(int id)
        {
            return _session.QueryOver<Category>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public List<Category> GetRootCategories()
        {
            var categoryContainer = _documentService.GetUniquePage<CategoryContainer>();
            return categoryContainer.PublishedChildren.OfType<Category>().ToList();
        }

        public CategorySearchModel GetCategoriesForSearch(int? categoryId)
        {
            if (!categoryId.HasValue)
                return GetRootCategoryModel();

            var category = _session.Get<Category>(categoryId);
            var categories = category.PublishedChildren.OfType<Category>().ToList();
            var hierarchy = category.ActivePages.OfType<Category>().ToList();
            hierarchy.Reverse();
            return new CategorySearchModel
                       {
                           Children = categories,
                           Hierarchy = hierarchy
                       };
        }

        private CategorySearchModel GetRootCategoryModel()
        {
            var categoryContainer = _documentService.GetUniquePage<CategoryContainer>();
            var categories = categoryContainer.PublishedChildren.OfType<Category>().ToList();
            return new CategorySearchModel
                       {
                           Children = categories
                       };
        }

        public CategoryContainer GetSiteCategoryContainer()
        {
            IList<CategoryContainer> categoryContainers = _session.QueryOver<CategoryContainer>().Where(x => x.Site == _currentSite.Site).Cacheable().List();
            if (categoryContainers.Any())
                return _session.QueryOver<CategoryContainer>().Cacheable().List().First();
            else
                return null;
        }
    }
}