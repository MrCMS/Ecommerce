using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.NewsletterBuilder.ACL;
using MrCMS.Website;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Models
{
    public class NewsletterBuilderAdminMenuModel : IAdminMenuItem
    {
        private readonly UrlHelper _urlHelper;
        private SubMenu _children;

        public NewsletterBuilderAdminMenuModel(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string Text => "Newsletter Builder";

        public string IconClass => "fa fa-envelope-o";

        public string Url => "#";

        public bool CanShow => CurrentRequestData.CurrentUser.CanAccess<NewsletterBuilderAdminMenuACL>(NewsletterBuilderAdminMenuACL.Show);

        public SubMenu Children
        {
            get { return _children = _children ?? GetChildren(); }
        }

        public int DisplayOrder => 60;

        private SubMenu GetChildren()
        {
            return new SubMenu
            {
                new ChildMenuItem("Templates", _urlHelper.Action("Index", "NewsletterTemplate"),
                    ACLOption.Create(new NewsletterTemplateACL(), NewsletterTemplateACL.List)),
                new ChildMenuItem("Newsletter", _urlHelper.Action("Index", "Newsletter"),
                    ACLOption.Create(new NewsletterACL(), NewsletterACL.List))
            };
        }
    }
}