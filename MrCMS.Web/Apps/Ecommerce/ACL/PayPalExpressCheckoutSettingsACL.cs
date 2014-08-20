using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class PayPalExpressCheckoutSettingsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "PayPal Express Checkout Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View };
        }
    }
}