using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceToolsMenuModel : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "Tools & Reports"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return true; } }
        public SubMenu Children
        {
            get
            {
                return _children ??
                       (_children = new SubMenu
                       {
                           {
                               "Tools",
                               new List<ChildMenuItem>
                               {
                                   new ChildMenuItem("Import/Export Products",
                                       "/Admin/Apps/Ecommerce/ImportExport/Products"),
                                   new ChildMenuItem("Google Base Integration",
                                       "/Admin/Apps/Ecommerce/GoogleBase/Dashboard"),
                                   new ChildMenuItem("Low Stock Report",
                                       "/Admin/Apps/Ecommerce/Stock/LowStockReport"),
                                   new ChildMenuItem("Bulk Stock Update",
                                       "/Admin/Apps/Ecommerce/Stock/BulkStockUpdate"),
                                   new ChildMenuItem("Bulk Shipping Update",
                                       "/Admin/Apps/Ecommerce/BulkShipping/Update"),
                               }
                           },
                           {
                               "Sales Reports",
                               new List<ChildMenuItem>
                               {
                                   new ChildMenuItem("Sales by day",
                                       "/Admin/Apps/Ecommerce/Report/SalesByDay"),
                                   new ChildMenuItem("Sales by payment type",
                                       "/Admin/Apps/Ecommerce/Report/SalesByPaymentType"),
                                   new ChildMenuItem("Sales by shipping type",
                                       "/Admin/Apps/Ecommerce/Report/SalesByShippingType"),
                               }
                           },
                           {
                               "Orders Reports",
                               new List<ChildMenuItem>
                               {
                                   new ChildMenuItem("Orders by shipping type",
                                       "/Admin/Apps/Ecommerce/Report/OrdersByShippingType"),
                               }
                           }
                       });
            }
        }
        public int DisplayOrder { get { return 51; } }
    }
}