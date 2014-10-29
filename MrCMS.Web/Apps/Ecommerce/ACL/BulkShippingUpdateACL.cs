using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class BulkShippingUpdateACL : ACLRule
    {
        public const string Update = "Update";

        public override string DisplayName
        {
            get { return "Bulk Shipping Update"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { Update };
        }
    }
}