using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Microsoft.Ajax.Utilities;
using MrCMS.Entities.People;
using MrCMS.Web.Areas.Admin.Models.UserEdit;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models.UserTabs
{
    public class OrdersTab :UserTab
    {
        public override int Order
        {
            get { return 0; }
        }

        public override string Name(User user)
        {
            return "Orders";
        }

        public override bool ShouldShow(User user)
        {
            return true;
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override string TabHtmlId
        {
            get { return "user-orders"; }
        }

        public override void RenderTabPane(HtmlHelper<User> html, User user)
        {
            html.RenderAction("List", "UserOrder", new {user});
        }
    }
}