using System.Web.Mvc;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Stats.Areas.Admin.Models
{
    public class PageStatsMenuModel : IAdminMenuItem
    {
        private readonly UrlHelper _urlHelper;

        public PageStatsMenuModel(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        private SubMenu _children;
        public string Text { get { return "Stats"; } }

        public string IconClass
        {
            get { return "fa fa-line-chart"; }
        }

        public string Url { get; private set; }
        public bool CanShow { get { return true; } }

        public SubMenu Children
        {
            get
            {
                return _children ??
                       (_children = GetChildren());
            }
        }

        private SubMenu GetChildren()
        {
            var subMenu = new SubMenu
            {
                new ChildMenuItem("Page Views", _urlHelper.Action("Index", "PageViews")),
            };

            return subMenu;

        }

        public int DisplayOrder { get { return 50; } }
    }
}