using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class LowStockReportACL : ACLRule
    {
        public const string View = "View";
        public const string CanExportLowStockReport = "Can Export Low Stock Report";
        public const string LowStockReportProductVariants = "Low Stock Report Product Variants";
        public const string Update = "Update";
        public const string ExportStockReport = "Can Export Stock Report";
        
        public override string DisplayName
        {
            get { return "Low Stock Report"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View, CanExportLowStockReport, LowStockReportProductVariants, Update, ExportStockReport };
        }
    }

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