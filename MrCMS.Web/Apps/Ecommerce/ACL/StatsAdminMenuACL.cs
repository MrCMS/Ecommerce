using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class StatsAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";
        public const string PageViews = "Page Views";
        public const string UserStats = "User Stats";

        public override string DisplayName
        {
            get { return "Stats Admin Menu"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
            {
                ShowMenu,
                PageViews,
                UserStats
            };
        }
    }
}