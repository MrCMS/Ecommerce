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
            var categoryContainer =
                session.QueryOver<CategoryContainer>()
                       .Where(x => x.Site == CurrentRequestData.CurrentSite)
                       .Cacheable().SingleOrDefault();
            var navigationRecords =
                session.QueryOver<Category>().Where(webpage => webpage.Parent != null && webpage.Parent.Id == categoryContainer.Id && webpage.PublishOn != null &&
                        webpage.PublishOn <= CurrentRequestData.Now && webpage.RevealInNavigation && webpage.Site == Site).Cacheable()
                       .List().OrderBy(webpage => webpage.DisplayOrder)
                       .Select(webpage => new NavigationRecord
                       {
                           Text = MvcHtmlString.Create(webpage.Name),
                           Url = MvcHtmlString.Create("/" + webpage.LiveUrlSegment),
                           Children = GetChildCategories(webpage, 2)
                       }).ToList();

            return new NavigationList(navigationRecords.ToList());
        }

        protected virtual List<NavigationRecord> GetChildCategories(Webpage entity, int nextLevel)
        {
            var navigation = new List<NavigationRecord>();
            if (nextLevel <= NoOfMenuLevels && entity.PublishedChildren.Any())
            {
                navigation.AddRange(entity.PublishedChildren.Select(item => new NavigationRecord
                        {
                            Text = MvcHtmlString.Create(item.Name),
                            Url = MvcHtmlString.Create("/" + item.UrlSegment),
                            Children = GetChildCategories(item, nextLevel + 1)
                        }));
            }
            return navigation;
        }
    }
}
