using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class ExportOrderACL : ACLRule
    {
        
        public const string ExportOrderToPdf = "ExportOrderToPdf";

        public override string DisplayName
        {
            get { return "Order Export"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { ExportOrderToPdf };
        }
    }
}