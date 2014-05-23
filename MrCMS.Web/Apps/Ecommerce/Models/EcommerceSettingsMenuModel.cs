using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceSettingsMenuModel : IAdminMenuItem
    {
        private IDictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "e-Settings"; } }
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
                                            "General settings",
                                            new List<IMenuItem>
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
                                            new List<IMenuItem>
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