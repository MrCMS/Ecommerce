using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class SagePaySettingsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "SagePay Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View };
        }
    }
}