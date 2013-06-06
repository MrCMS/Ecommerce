using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceAdminMenuModel : IAdminMenuItem
    {
        private IDictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "Ecommerce"; } }
        public string Url { get; private set; }
        public IDictionary<string, List<IMenuItem>> Children
        {
            get
            {
                return _children ??
                       (_children = new Dictionary<string, List<IMenuItem>>
                                        {
                                            {
                                                "Admin",
                                                new List<IMenuItem>
                                                    {
                                                        new ChildMenuItem("Products", "/Admin/Apps/Ecommerce/Product"),
                                                        new ChildMenuItem("Categories", "/Admin/Apps/Ecommerce/Category"),
                                                        new ChildMenuItem("Brands", "/Admin/Apps/Ecommerce/Brand"),
                                                        new ChildMenuItem("Product Specification Attributes",
                                                                          "/Admin/Apps/Ecommerce/ProductSpecificationAttribute"),
                                                        new ChildMenuItem("Discounts", "/Admin/Apps/Ecommerce/Discount"),
                                                        new ChildMenuItem("Orders", "/Admin/Apps/Ecommerce/Order"),
                                                    }
                                            },
                                            {
                                                "Tools",
                                                new List<IMenuItem>
                                                    {
                                                        new ChildMenuItem("Import/Export Categories",
                                                                          "/Admin/Apps/Ecommerce/Category/ImportExport"),
                                                        new ChildMenuItem("Import/Export Products",
                                                                          "/Admin/Apps/Ecommerce/Product/ImportExport"),
                                                        new ChildMenuItem("Import/Export Product Images",
                                                                          "/Admin/Apps/Ecommerce/ProductImage/ImportExport"),
                                                        new ChildMenuItem("Google Base Integration",
                                                                          "/Admin/Apps/Ecommerce/GoogleBase"),
                                                    }
                                            },
                                            {
                                                "Settings",
                                                new List<IMenuItem>
                                                    {
                                                         new ChildMenuItem("Global Settings",
                                                                          "/Admin/Apps/Ecommerce/EcommerceSettings/Edit"),
                                                        new ChildMenuItem("Geographic Data",
                                                                          "/Admin/Apps/Ecommerce/Country"),
                                                        new ChildMenuItem("Taxes", "/Admin/Apps/Ecommerce/TaxRate"),
                                                        new ChildMenuItem("Notification Template Settings",
                                                                          "/Admin/Apps/Ecommerce/NotificationTemplateSettings/Edit"),
                                                        new ChildMenuItem("Shipping Methods",
                                                                          "/Admin/Apps/Ecommerce/ShippingMethod"),
                                                        new ChildMenuItem("Shipping Calculations",
                                                                          "/Admin/Apps/Ecommerce/ShippingCalculation"),
                                                    }
                                            }
                                        });
            }
        }
        public int DisplayOrder { get { return 50; } }
    }
}