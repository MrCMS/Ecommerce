using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class DashboardRevenueACL : ACLRule
    {
        public const string ShowRevenue = "Show Revenue";

        public override string DisplayName
        {
            get { return "Revenue Dashboard"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { ShowRevenue };
        }
    }
}