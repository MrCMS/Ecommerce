using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class OrderACL : ACLRule
    {
        public const string List = "List";
        public const string Edit = "Edit";
        public const string Cancel = "Cancel";
        public const string Delete = "Delete";
        public const string MarkAsShipped = "MarkAsShipped";
        public const string MarkAsPaid = "MarkAsPaid";
        public const string MarkAsVoided = "MarkAsVoided";
        public const string BulkShippingUpdate = "BulkShippingUpdate";
        public const string SetTrackingNumber = "SetTrackingNumber";
        public const string ExportOrderToPdf = "ExportOrderToPdf";
        
        public override string DisplayName
        {
            get { return "Order"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { List, Edit, Cancel, Delete, MarkAsShipped, MarkAsPaid, MarkAsVoided, BulkShippingUpdate, SetTrackingNumber, ExportOrderToPdf };
        }
    }
}