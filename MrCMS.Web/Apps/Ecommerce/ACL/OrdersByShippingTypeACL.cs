using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class OrdersByShippingTypeACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "Orders By Shipping Type"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View };
        }
    }
}