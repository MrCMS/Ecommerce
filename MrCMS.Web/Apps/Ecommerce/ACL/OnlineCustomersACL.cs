using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class OnlineCustomersACL : ACLRule
    {
        public const string ViewCustomers = "View Customers";
        public const string ViewCart = "View Cart";

        public override string DisplayName
        {
            get { return "Online Customers"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>
            {
                ViewCustomers,
                ViewCart
            };
        }
    }
}