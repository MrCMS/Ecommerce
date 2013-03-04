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

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;

        public CategoryService(ISession session, IDocumentService documentService)
        {
            _session = session;
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
    }
}