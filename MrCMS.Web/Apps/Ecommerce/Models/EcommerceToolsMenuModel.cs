using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceToolsMenuModel : IAdminMenuItem
    {
        private readonly UrlHelper _urlHelper;

        public EcommerceToolsMenuModel(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        private SubMenu _children;
        public string Text { get { return "Tools & Reports"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return CurrentRequestData.CurrentUser.CanAccess<ToolsAndReportsACL>(ToolsAndReportsACL.Show); } }
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
                                       _urlHelper.Action("Products","ImportExport"),
                                       ACLOption.Create(new ImportExportACL(), ImportExportACL.View)),
                                   new ChildMenuItem("Google Base Integration",
                                       _urlHelper.Action("Dashboard","GoogleBase"),
                                       ACLOption.Create(new GoogleBaseACL(), GoogleBaseACL.View)),
                                   new ChildMenuItem("Low Stock Report",
                                       _urlHelper.Action("Index","LowStockReport"),
                                       ACLOption.Create(new LowStockReportACL(), LowStockReportACL.View)),
                                   new ChildMenuItem("Bulk Stock Update",
                                       _urlHelper.Action("Index","BulkStockUpdate"),
                                       //"/Admin/Apps/Ecommerce/Stock/BulkStockUpdate",
                                       ACLOption.Create(new BulkStockUpdateACL(), BulkStockUpdateACL.BulkStockUpdate)),
                                   new ChildMenuItem("Bulk Shipping Update",
                                       "/Admin/Apps/Ecommerce/BulkShipping/Update", ACLOption.Create(new BulkShippingUpdateACL(), BulkShippingUpdateACL.Update)),
                               }
                           },
                           {
                               "Sales Reports",
                               new List<ChildMenuItem>
                               {
                                   new ChildMenuItem("Sales by day",
                                       "/Admin/Apps/Ecommerce/Report/SalesByDay", ACLOption.Create(new SalesByDayACL(), SalesByDayACL.View)),
                                   new ChildMenuItem("Sales by payment type",
                                       "/Admin/Apps/Ecommerce/Report/SalesByPaymentType", ACLOption.Create(new SalesByPaymentTypeACL(), SalesByPaymentTypeACL.View)),
                                   new ChildMenuItem("Sales by shipping type",
                                       "/Admin/Apps/Ecommerce/Report/SalesByShippingType", ACLOption.Create(new SalesByShippingTypeACL(), SalesByShippingTypeACL.View)),
                               }
                           },
                           {
                               "Orders Reports",
                               new List<ChildMenuItem>
                               {
                                   new ChildMenuItem("Orders by shipping type",
                                       "/Admin/Apps/Ecommerce/Report/OrdersByShippingType", ACLOption.Create(new OrdersByShippingTypeACL(), OrdersByShippingTypeACL.View)),
                               }
                           }
                       });
            }
        }
        public int DisplayOrder { get { return 51; } }
    }
}