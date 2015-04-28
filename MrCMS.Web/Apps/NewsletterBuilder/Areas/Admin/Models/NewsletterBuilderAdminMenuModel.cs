using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.NewsletterBuilder.ACL;

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

        public string Text
        {
            get { return "Newsletter Builder"; }
        }

        public string IconClass
        {
            get { return "fa fa-envelope-o"; }
        }

        public string Url
        {
            get { return "#"; }
        }

        public bool CanShow
        {
            get { return true; }
        }

        public SubMenu Children
        {
            get { return _children = _children ?? GetChildren(); }
        }

        public int DisplayOrder
        {
            get { return 60; }
        }

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