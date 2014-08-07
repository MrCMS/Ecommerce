using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceReportsMenuModel : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "e-Reports"; } }
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
                               "Sales",
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
                               "Orders",
                               new List<ChildMenuItem>
                               {
                                   new ChildMenuItem("Orders by shipping type",
                                       "/Admin/Apps/Ecommerce/Report/OrdersByShippingType"),
                               }
                           }
                       });
            }
        }
        public int DisplayOrder { get { return 52; } }
    }
}