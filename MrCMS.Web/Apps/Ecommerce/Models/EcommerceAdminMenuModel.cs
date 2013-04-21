using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceAdminMenuModel : IAdminMenuItem
    {
        private List<IMenuItem> _children;
        public string Text { get { return "Ecommerce"; } }
        public string Url { get; private set; }
        public List<IMenuItem> Children
        {
            get
            {
                return _children ??
                       (_children =
                        new List<IMenuItem>
                        {
                            new ChildMenuItem("Products", "/Admin/Apps/Ecommerce/Product"),
                            new ChildMenuItem("Categories", "/Admin/Apps/Ecommerce/Category"),
                            new ChildMenuItem("Brands", "/Admin/Apps/Ecommerce/Brand"),
                            new ChildMenuItem("Geographic Data", "/Admin/Apps/Ecommerce/Country"),
                            new ChildMenuItem("Tax Rates", "/Admin/Apps/Ecommerce/TaxRate"),
                            new ChildMenuItem("Product Specification Options", "/Admin/Apps/Ecommerce/ProductSpecificationOption"),
                            new ChildMenuItem("Shipping Methods", "/Admin/Apps/Ecommerce/ShippingMethod"),
                            new ChildMenuItem("Shipping Calculations", "/Admin/Apps/Ecommerce/ShippingCalculation"),
                            new ChildMenuItem("Discounts", "/Admin/Apps/Ecommerce/Discount"),
                            new ChildMenuItem("Notification Template Settings", "/Admin/Apps/Ecommerce/NotificationTemplateSettings/Edit"),
                            new ChildMenuItem("Orders", "/Admin/Apps/Ecommerce/Order"),
                        });
            }
        }
        public int DisplayOrder { get { return 50; } }
    }
}