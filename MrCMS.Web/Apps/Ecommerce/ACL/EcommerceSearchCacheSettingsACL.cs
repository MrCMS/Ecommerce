using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class EcommerceSearchCacheSettingsACL : ACLRule
    {
        public const string Edit = "Edit";

        public override string DisplayName
        {
            get { return "Ecommerce Search Cache Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { Edit };
        }
    }
}