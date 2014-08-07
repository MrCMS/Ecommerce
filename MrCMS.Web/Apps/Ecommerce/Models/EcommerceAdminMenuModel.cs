using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceAdminMenuModel : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "Catalog"; } }
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
                                                "Admin",
                                                new List<ChildMenuItem>
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
                                                new List<ChildMenuItem>
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
}