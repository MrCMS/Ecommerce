using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.People;
using MrCMS.Web.Areas.Admin.Models.UserEdit;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models.UserTabs
{
    public class RewardPointsTab : UserTab
    {
        public override int Order
        {
            get { return 1000; }
        }

        public override string Name(User user)
        {
            return "Reward points";
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
            get { return "user-reward-points"; }
        }

        public override void RenderTabPane(HtmlHelper<User> html, User user)
        {
            html.RenderAction("List", "UserRewardPoints", new { user });
        }
    }
}