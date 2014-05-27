using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceAdminMenuModel : IAdminMenuItem
    {
        private IDictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "e-Catalog"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return true; } }
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
                                                        new ChildMenuItem("Orders", "/Admin/Apps/Ecommerce/Order"),
                                                        new ChildMenuItem("Products", "/Admin/Apps/Ecommerce/Product"),
                                                        new ChildMenuItem("Categories", "/Admin/Apps/Ecommerce/Category"),
                                                        new ChildMenuItem("Brands", "/Admin/Apps/Ecommerce/Brand"),
                                                        new ChildMenuItem("Product Specifications",
                                                                          "/Admin/Apps/Ecommerce/ProductSpecificationAttribute"),
                                                        new ChildMenuItem("Discounts", "/Admin/Apps/Ecommerce/Discount"),
                                                        
                                                    }
                                            },
                                            {
                                                "Newsletter Builder",
                                                new List<IMenuItem>
                                                    {
                                                        new ChildMenuItem("Templates", "/Admin/Apps/Ecommerce/NewsletterTemplate"),
                                                        new ChildMenuItem("Newsletter", "/Admin/Apps/Ecommerce/Newsletter")
                                                    }
                                            }
                                        });
            }
        }
        public int DisplayOrder { get { return 50; } }
    }

    public class EcommerceToolsMenuModel : IAdminMenuItem
    {
        private IDictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "e-Tools"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return true; } }
        public IDictionary<string, List<IMenuItem>> Children
        {
            get
            {
                return _children ??
                       (_children = new Dictionary<string, List<IMenuItem>>
                                        {
                                            {
                                                "Tools",
                                                new List<IMenuItem>
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
                                                                          "/Admin/Apps/Ecommerce/Order/BulkShippingUpdate"),
                                                    }
                                            }
                                        });
            }
        }
        public int DisplayOrder { get { return 51; } }
    }

    public class EcommerceReportsMenuModel : IAdminMenuItem
    {
        private IDictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "e-Reports"; } }
        public string Url { get; private set; }
        public bool CanShow { get { return true; } }
        public IDictionary<string, List<IMenuItem>> Children
        {
            get
            {
                return _children ??
                       (_children = new Dictionary<string, List<IMenuItem>>
                                        {
                                            {
                                                "Sales",
                                                new List<IMenuItem>
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
                                                new List<IMenuItem>
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