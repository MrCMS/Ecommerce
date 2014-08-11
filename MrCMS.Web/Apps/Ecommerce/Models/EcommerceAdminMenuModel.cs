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
                                            },
                                            {
                                                "Settings",
                                                new List<ChildMenuItem>
                                                    {
                                                        new ChildMenuItem("General Settings","#",subMenu: new SubMenu
                                                           {
                                                               {"",new List<ChildMenuItem>
                                                               {
                                                            new ChildMenuItem("Global Settings",
                                                                "/Admin/Apps/Ecommerce/EcommerceSettings/Edit"),
                                                            new ChildMenuItem("Currencies", "/Admin/Apps/Ecommerce/Currency"),
                                                            new ChildMenuItem("Geographic Data",
                                                                "/Admin/Apps/Ecommerce/Country"),
                                                            new ChildMenuItem("Taxes", "/Admin/Apps/Ecommerce/TaxRate"),
                                                            new ChildMenuItem("Shipping Methods",
                                                                "/Admin/Apps/Ecommerce/ShippingMethod"),
                                                            new ChildMenuItem("Shipping Calculations",
                                                                "/Admin/Apps/Ecommerce/ShippingCalculation"),
                                                               }}
                                                           }),
                                                           new ChildMenuItem("Payment Settings","#",subMenu: new SubMenu
                                                           {
                                                               {"",new List<ChildMenuItem>
                                                               {
                                                           new ChildMenuItem("Payment Settings",
                                                                "/Admin/Apps/Ecommerce/PaymentSettings"),
                                                            new ChildMenuItem("Paypoint",
                                                                "/Admin/Apps/Ecommerce/PaypointSettings"),
                                                            new ChildMenuItem("PayPal Express Checkout",
                                                                "/Admin/Apps/Ecommerce/PayPalExpressCheckoutSettings"),
                                                            new ChildMenuItem("SagePay",
                                                                "/Admin/Apps/Ecommerce/SagePaySettings"),
                                                            new ChildMenuItem("WorldPay",
                                                                "/Admin/Apps/Ecommerce/WorldPaySettings"),
                                                               }}
                                                           }),
                                                    }
                                            }
                                        });
            }
        }
        public int DisplayOrder { get { return 50; } }
    }
}