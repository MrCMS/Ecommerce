using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Stats.Areas.Admin.Models
{
    public class PageStatsMenuModel : IAdminMenuItem
    {
        private readonly UrlHelper _urlHelper;

        private SubMenu _children;

        public string Text { get { return "Stats"; } }

        public string IconClass
        {
            get { return "fa fa-line-chart"; }
        }

        public string Url { get; private set; }

        public bool CanShow { get { return CurrentRequestData.CurrentUser.CanAccess<StatsAdminMenuACL>(StatsAdminMenuACL.ShowMenu); } }

        public SubMenu Children
        {
            get
            {
                return _children ?? (_children = GetChildren());
            }
        }

        public int DisplayOrder { get { return 50; } }

        public PageStatsMenuModel(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        private SubMenu GetChildren()
        {
            var subMenu = new SubMenu
            {
                new ChildMenuItem("Page Views", _urlHelper.Action("Index", "PageViews"), ACLOption.Create(new StatsAdminMenuACL(), StatsAdminMenuACL.PageViews))
            };

            return subMenu;
        }
    }
}