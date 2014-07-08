using System.ComponentModel;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using NHibernate;
using System.Web.Mvc;
using System.Linq;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class CategoriesOnlyNavigation : Widget
    {
        [DisplayName("Max. number of levels for display in menu")]
        public virtual int NoOfMenuLevels { get; set; }

        public override bool HasProperties
        {
            get { return false; }
        }

        public override object GetModel(ISession session)
        {
            var productSearch =
                session.QueryOver<ProductSearch>()
                       .Cacheable().SingleOrDefault();

            var categories = session.QueryOver<Category>().Cacheable().List();

            var navigationRecords =
                categories.Where(webpage => webpage.Parent != null && webpage.Parent.Id == productSearch.Id && webpage.PublishOn != null &&
                        webpage.PublishOn <= CurrentRequestData.Now && webpage.RevealInNavigation && webpage.Site == Site)
                       .OrderBy(webpage => webpage.DisplayOrder)
                       .Select(webpage => new NavigationRecord
                       {
                           Text = MvcHtmlString.Create(webpage.Name),
                           Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment),
                           Children = GetChildCategories(webpage, 2, categories)
                       }).ToList();

            return new NavigationList(navigationRecords.ToList());
        }

        protected virtual List<NavigationRecord> GetChildCategories(Webpage entity, int maxLevel, IList<Category> categories)
        {
            var navigation = new List<NavigationRecord>();
            if (maxLevel > NoOfMenuLevels) return navigation;
            var publishedChildren =
                categories
                    .Where(webpage => webpage.Parent.Id == entity.Id && webpage.PublishOn != null)
                    .Where(webpage => webpage.Published).ToList();
            if (publishedChildren.Any())
            {
                navigation.AddRange(publishedChildren.Select(item => new NavigationRecord
                {
                    Text =
                        MvcHtmlString.Create(item.Name),
                    Url =
                        MvcHtmlString.Create("/" +
                                             item
                        .UrlSegment),
                    Children =
                        GetChildCategories(item,
                            maxLevel + 1, categories)
                }));
            }
            return navigation;
        }
    }
}
