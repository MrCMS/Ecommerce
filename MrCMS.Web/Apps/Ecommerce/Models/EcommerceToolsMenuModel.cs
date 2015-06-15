using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
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
        public string Text { get { return "Tools"; } }
        public string IconClass { get { return "fa fa-wrench"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return CurrentRequestData.CurrentUser.CanAccess<ToolsAndReportsACL>(ToolsAndReportsACL.Show); } }
        public SubMenu Children
        {
            get
            {
                return _children ??
                       (_children = new SubMenu
                       {
                           new ChildMenuItem("Import/Export Products",
                               _urlHelper.Action("Products", "ImportExport"),
                               ACLOption.Create(new ImportExportACL(), ImportExportACL.View)),
                           new ChildMenuItem("NopCommerce Import",
                               _urlHelper.Action("Index", "NopDataImport"),
                               ACLOption.Create(new ImportExportACL(), ImportExportACL.View)),
                           new ChildMenuItem("Order Export",
                               _urlHelper.Action("Index", "OrderExport"),
                               ACLOption.Create(new ImportExportACL(), ImportExportACL.View)),
                           new ChildMenuItem("Google Base Integration",
                               _urlHelper.Action("Dashboard", "GoogleBase"),
                               ACLOption.Create(new GoogleBaseACL(), GoogleBaseACL.View)),
                           new ChildMenuItem("Low Stock Report",
                               _urlHelper.Action("Index", "LowStockReport"),
                               ACLOption.Create(new LowStockReportACL(), LowStockReportACL.View)),
                           new ChildMenuItem("Bulk Stock Update",
                               _urlHelper.Action("Index", "BulkStockUpdate"),
                               ACLOption.Create(new BulkStockUpdateACL(), BulkStockUpdateACL.BulkStockUpdate)),
                           new ChildMenuItem("Bulk Shipping Update",
                               _urlHelper.Action("Update","BulkShipping"),
                               ACLOption.Create(new BulkShippingUpdateACL(), BulkShippingUpdateACL.Update)),
                       });
            }
        }
        public int DisplayOrder { get { return 48; } }
    }
}

