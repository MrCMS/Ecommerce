using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class LowStockReportACL : ACLRule
    {
        public const string View = "View";
        public const string CanExport = "Can Export";
        public const string Update = "Update";

        public override string DisplayName
        {
            get { return "Low Stock Report"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View, CanExport, Update };
        }
    }
}