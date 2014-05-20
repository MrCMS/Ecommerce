using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class CategoryAdminService : ICategoryAdminService
    {
        private readonly ISession _session;
        private readonly IUniquePageService _uniquePageService;

        public CategoryAdminService(ISession session, IUniquePageService uniquePageService)
        {
            _session = session;
            _uniquePageService = uniquePageService;
        }

        public CategoryPagedList Search(string queryTerm = null, int page = 1)
        {
            IPagedList<Category> pagedList;

            if (!string.IsNullOrWhiteSpace(queryTerm))
            {
                pagedList =
                    _session.Paged(
                        QueryOver.Of<Category>()
                            .Where(category => category.Name.IsInsensitiveLike(queryTerm, MatchMode.Anywhere)), page);
            }
            else
            {
                pagedList = _session.Paged(QueryOver.Of<Category>(), page);
            }

            var categoryContainer = _uniquePageService.GetUniquePage<CategoryContainer>();
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


        public bool CategoryContainerExists()
        {
            return _uniquePageService.GetUniquePage<CategoryContainer>() != null;
        }

        public List<ProductSpecificationAttribute> GetShownSpecifications(Category category)
        {
            return _session.QueryOver<ProductSpecificationAttribute>()
                .Cacheable()
                .List()
                .Where(attribute => !category.HiddenSearchSpecifications.Contains(attribute))
                .ToList();
        }

        public bool AddSpecificationToHidden(ProductSpecificationAttribute attribute, int categoryId)
        {
            var category = _session.Get<Category>(categoryId);

            if (category == null)
                return false;
            category.HiddenSearchSpecifications.Add(attribute);
            attribute.HiddenInCategories.Add(category);
            _session.Transact(session => session.Update(category));
            return true;
        }

        public bool RemoveSpecificationFromHidden(ProductSpecificationAttribute attribute, int categoryId)
        {
            var category = _session.Get<Category>(categoryId);

            if (category == null)
                return false;
            category.HiddenSearchSpecifications.Remove(attribute);
            attribute.HiddenInCategories.Remove(category);
            _session.Transact(session => session.Update(category));
            return true;
        }
    }
}