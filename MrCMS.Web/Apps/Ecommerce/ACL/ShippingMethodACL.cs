using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class ShippingMethodACL : ACLRule
    {
        public const string List = "List";

        public override string DisplayName
        {
            get { return "Shipping Method"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { List };
        }
    }
}