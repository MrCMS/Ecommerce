using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.ACLRules;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Models
{
    public class CustomerFeedbackAdminMenu : IAdminMenuItem
    {
        private readonly UrlHelper _urlHelper;
        private SubMenu _children;
        public CustomerFeedbackAdminMenu(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }
        public string Text { get { return "Customer Feedback"; } }
        public string IconClass { get { return "fa fa-thumbs-o-up"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return true; } }
        public SubMenu Children { get { return _children = _children ?? GetChildren(); } }
        public int DisplayOrder { get { return 61; } }
        private SubMenu GetChildren()
        {
            return new SubMenu
            {
                new ChildMenuItem("Settings", _urlHelper.Action("Index", "CustomerFeedbackSettings"),
                    ACLOption.Create(new CustomerFeedbackSettingsACL(), CustomerFeedbackSettingsACL.View)),
                new ChildMenuItem("Order Feedback Facets", _urlHelper.Action("Index", "FeedbackFacet")),
                new ChildMenuItem("Order Feedback", _urlHelper.Action("Index", "FeedbackRecord"))
            };
        }
    }
}