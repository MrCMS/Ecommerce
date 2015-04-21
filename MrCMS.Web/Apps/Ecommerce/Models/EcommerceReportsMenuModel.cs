using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceReportsMenuModel : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "Reports"; } }
        public string IconClass { get { return "fa fa-pie-chart"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return CurrentRequestData.CurrentUser.CanAccess<ToolsAndReportsACL>(ToolsAndReportsACL.Show); } }
        public SubMenu Children
        {
            get
            {
                return _children ??
                       (_children = new SubMenu
                       {
                           new ChildMenuItem("Sales by day",
                               "/Admin/Apps/Ecommerce/Report/SalesByDay", ACLOption.Create(new SalesByDayACL(), SalesByDayACL.View)),
                           new ChildMenuItem("Sales by payment type",
                               "/Admin/Apps/Ecommerce/Report/SalesByPaymentType", ACLOption.Create(new SalesByPaymentTypeACL(), SalesByPaymentTypeACL.View)),
                           new ChildMenuItem("Sales by shipping type",
                               "/Admin/Apps/Ecommerce/Report/SalesByShippingType", ACLOption.Create(new SalesByShippingTypeACL(), SalesByShippingTypeACL.View)),
                           new ChildMenuItem("Orders by shipping type",
                               "/Admin/Apps/Ecommerce/Report/OrdersByShippingType", ACLOption.Create(new OrdersByShippingTypeACL(), OrdersByShippingTypeACL.View)),

                       });
            }
        }
        public int DisplayOrder { get { return 50; } }
    }
}