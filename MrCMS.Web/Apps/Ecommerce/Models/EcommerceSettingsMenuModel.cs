using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceSettingsMenuModel : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text { get { return "Settings"; } }
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
                                            "General settings",
                                            new List<ChildMenuItem>
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
                                            }
                                        },
                                        {
                                            "Payment settings",
                                            new List<ChildMenuItem>
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
                                            }
                                        }
                                    });
            }
        }
        public int DisplayOrder { get { return 54; } }
    }
}