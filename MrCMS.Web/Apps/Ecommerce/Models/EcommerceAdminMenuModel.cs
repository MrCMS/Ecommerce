using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceAdminMenuModel : IAdminMenuItem
    {
        private readonly IShippingMethodSubmenuGenerator _shippingMethodSubmenuGenerator;
        private readonly EcommerceSettings _ecommerceSettings;

        public EcommerceAdminMenuModel(IShippingMethodSubmenuGenerator shippingMethodSubmenuGenerator,
             EcommerceSettings ecommerceSettings)
        {
            _shippingMethodSubmenuGenerator = shippingMethodSubmenuGenerator;
            _ecommerceSettings = ecommerceSettings;
        }

        public string Text
        {
            get { return "Catalog"; }
        }

        public string Url { get; private set; }

        public bool CanShow
        {
            get { return true; }
        }

        public SubMenu Children
        {
            get
            {
                var adminItems = new List<ChildMenuItem>
                {
                    new ChildMenuItem("Orders", "/Admin/Apps/Ecommerce/Order"),
                    new ChildMenuItem("Products", "/Admin/Apps/Ecommerce/Product"),
                    new ChildMenuItem("Categories", "/Admin/Apps/Ecommerce/Category"),
                    new ChildMenuItem("Brands", "/Admin/Apps/Ecommerce/Brand"),
                    new ChildMenuItem("Product Specifications", "/Admin/Apps/Ecommerce/ProductSpecificationAttribute"),
                    new ChildMenuItem("Discounts", "/Admin/Apps/Ecommerce/Discount"),
                };
                if (_ecommerceSettings.GiftCardsEnabled)
                {
                    adminItems.Add(new ChildMenuItem("Gift Cards", "/Admin/Apps/Ecommerce/GiftCard"));
                }
                return new SubMenu
                       {
                           {
                               "Admin",
                               adminItems
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
                                   new ChildMenuItem("General Settings", "#", subMenu: new SubMenu
                                   {
                                       {
                                           "", new List<ChildMenuItem>
                                           {
                                               new ChildMenuItem("Global Settings", "/Admin/Apps/Ecommerce/EcommerceSettings/Edit"),
                                               new ChildMenuItem("Search Cache Settings", "/Admin/Apps/Ecommerce/EcommerceSearchCacheSettings/Edit"),
                                               new ChildMenuItem("Currencies", "/Admin/Apps/Ecommerce/Currency"),
                                               new ChildMenuItem("Geographic Data", "/Admin/Apps/Ecommerce/Country"),
                                               new ChildMenuItem("Taxes", "/Admin/Apps/Ecommerce/TaxRate"),
                                               new ChildMenuItem("Shipping", "#", subMenu: _shippingMethodSubmenuGenerator.Get())
                                           }
                                       }
                                   }),
                                   new ChildMenuItem("Payment Settings", "#", subMenu: new SubMenu
                                   {
                                       {
                                           "", new List<ChildMenuItem>
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
                                   }),
                               }
                           }
                       };
            }
        }

        public int DisplayOrder
        {
            get { return 50; }
        }
    }
}