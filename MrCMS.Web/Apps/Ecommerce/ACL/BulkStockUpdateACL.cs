using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class BulkStockUpdateACL : ACLRule
    {
        public const string BulkStockUpdate = "Bulk Stock Update";

        public override string DisplayName
        {
            get { return "Bulk Stock Update Report"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { BulkStockUpdate };
        }
    }
}