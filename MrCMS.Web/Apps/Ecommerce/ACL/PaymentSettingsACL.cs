using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class PaymentSettingsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "Payment Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View };
        }
    }
}