using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Amazon.ACL
{
    public class AmazonAdminMenuACL : ACLRule
    {
        public const string ShowMenu = "Show Menu";
        public const string Dashboard = "Dashboard";
        public const string Listings = "Listings";
        public const string Orders = "Orders";
        public const string OrderSync = "OrderSync";
        public const string AmazonApp = "App";
        public const string Seller = "Seller";
        public const string Sync = "Sync";
        public const string Logs = "Logs";

        public override string DisplayName
        {
            get { return "Amazon Admin Menu"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
            {
                ShowMenu,
                Dashboard,
                Listings,
                Orders,
                OrderSync,
                AmazonApp,
                Seller,
                Sync,
                Logs
            };
        }
    }
}