using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetCategoriesOnlyNavModel : GetWidgetModelBase<CategoriesOnlyNavigation>
    {
        private readonly ISession _session;
        private readonly IUniquePageService _uniquePageService;

        public GetCategoriesOnlyNavModel(ISession session,IUniquePageService uniquePageService)
        {
            _session = session;
            _uniquePageService = uniquePageService;
        }

        public override object GetModel(CategoriesOnlyNavigation widget)
        {
            var productSearch = _uniquePageService.GetUniquePage<ProductSearch>();
            var categories = _session.QueryOver<Category>().Cacheable().List();

            var navigationRecords =
                categories.Where(webpage => webpage.Parent != null && webpage.Parent.Id == productSearch.Id && webpage.PublishOn != null &&
                                            webpage.PublishOn <= CurrentRequestData.Now && webpage.RevealInNavigation && webpage.Site.Id == widget.Site.Id)
                    .OrderBy(webpage => webpage.DisplayOrder)
                    .Select(webpage => new NavigationRecord
                    {
                        Text = MvcHtmlString.Create(webpage.Name),
                        Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment),
                        Children = GetChildCategories(webpage, widget, 2, categories)
                    }).ToList();

            return new NavigationList(navigationRecords.ToList());
        }

        protected virtual List<NavigationRecord> GetChildCategories(Webpage entity, CategoriesOnlyNavigation widget, int maxLevel, IList<Category> categories)
        {
            var navigation = new List<NavigationRecord>();
            if (maxLevel > widget.NoOfMenuLevels) return navigation;
            var publishedChildren =
                categories
                    .Where(webpage => webpage.Parent.Id == entity.Id && webpage.PublishOn != null)
                    .Where(webpage => webpage.Published).ToList();
            if (publishedChildren.Any())
            {
                navigation.AddRange(publishedChildren.Select(item => new NavigationRecord
                {
                    Text = MvcHtmlString.Create(item.Name),
                    Url = MvcHtmlString.Create("/" + item.UrlSegment),
                    Children = GetChildCategories(item, widget, maxLevel + 1, categories)
                }));
            }
            return navigation;
        }
    }
}