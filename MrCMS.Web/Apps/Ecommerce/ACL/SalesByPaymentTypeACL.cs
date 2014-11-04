using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class SalesByPaymentTypeACL : ACLRule
    {
        public const string View = "View";
        public override string DisplayName
        {
            get { return "Sales By Payment"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View };
        }
    }
}