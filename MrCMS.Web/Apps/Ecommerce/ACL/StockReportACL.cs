using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class StockReportACL : ACLRule
    {
        public const string CanExport = "Can Export";

        public override string DisplayName
        {
            get { return "Stock Report"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { CanExport };
        }
    }
}