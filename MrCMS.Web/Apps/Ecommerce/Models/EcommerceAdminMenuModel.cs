using System.Collections.Generic;
using System.Web.UI.WebControls;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

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
            get { return CurrentRequestData.CurrentUser.CanAccess<CatalogACL>(CatalogACL.Show); }
        }

        public SubMenu Children
        {
            get
            {
                var adminItems = new List<ChildMenuItem>
                {
                    new ChildMenuItem("Orders", "/Admin/Apps/Ecommerce/Order", ACLOption.Create(new OrderACL(), OrderACL.List)),
                    new ChildMenuItem("Products", "/Admin/Apps/Ecommerce/Product", ACLOption.Create(new ProductACL(), ProductACL.List)),
                    new ChildMenuItem("Categories", "/Admin/Apps/Ecommerce/Category", ACLOption.Create(new CategoryACL(), CategoryACL.List)),
                    new ChildMenuItem("Brands", "/Admin/Apps/Ecommerce/Brand", ACLOption.Create(new BrandACL(), BrandACL.List)),
                    new ChildMenuItem("Product Specifications", "/Admin/Apps/Ecommerce/ProductSpecificationAttribute", ACLOption.Create(new ProductSpecificationAttributeACL(), ProductSpecificationAttributeACL.List)),
                    new ChildMenuItem("Discounts", "/Admin/Apps/Ecommerce/Discount", ACLOption.Create(new DiscountACL(), DiscountACL.List)),
                };
                if (_ecommerceSettings.GiftCardsEnabled)
                {
                    adminItems.Add(new ChildMenuItem("Gift Cards", "/Admin/Apps/Ecommerce/GiftCard", ACLOption.Create(new GiftCardACL(), GiftCardACL.List)));
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
                                   new ChildMenuItem("Templates", "/Admin/Apps/Ecommerce/NewsletterTemplate", ACLOption.Create(new NewsletterTemplateACL(), NewsletterTemplateACL.List)),
                                   new ChildMenuItem("Newsletter", "/Admin/Apps/Ecommerce/Newsletter", ACLOption.Create(new NewsletterACL(), NewsletterACL.List))
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
                                               new ChildMenuItem("Global Settings", "/Admin/Apps/Ecommerce/EcommerceSettings/Edit", ACLOption.Create(new EcommerceSettingsACL(), EcommerceSettingsACL.Edit)),
                                               new ChildMenuItem("Search Cache Settings", "/Admin/Apps/Ecommerce/EcommerceSearchCacheSettings/Edit", ACLOption.Create(new EcommerceSearchCacheSettingsACL(), EcommerceSearchCacheSettingsACL.Edit)),
                                               new ChildMenuItem("Currencies", "/Admin/Apps/Ecommerce/Currency", ACLOption.Create(new CurrencyACL(), CurrencyACL.List)),
                                               new ChildMenuItem("Geographic Data", "/Admin/Apps/Ecommerce/Country", ACLOption.Create(new CountryACL(), CountryACL.List)),
                                               new ChildMenuItem("Taxes", "/Admin/Apps/Ecommerce/TaxRate", ACLOption.Create(new TaxRateACL(), TaxRateACL.List)),
                                               new ChildMenuItem("Shipping", "#", ACLOption.Create(new ShippingMethodACL(), ShippingMethodACL.List), subMenu: _shippingMethodSubmenuGenerator.Get())
                                           }
                                       }
                                   }),
                                   new ChildMenuItem("Payment Settings", "#", subMenu: new SubMenu
                                   {
                                       {
                                           "", new List<ChildMenuItem>
                                           {
                                               new ChildMenuItem("Payment Settings",
                                                   "/Admin/Apps/Ecommerce/PaymentSettings",  ACLOption.Create(new PaymentSettingsACL(), PaymentSettingsACL.View)),
                                               new ChildMenuItem("Paypoint",
                                                   "/Admin/Apps/Ecommerce/PaypointSettings",  ACLOption.Create(new PaypointSettingsACL(), PaypointSettingsACL.View)),
                                               new ChildMenuItem("PayPal Express Checkout",
                                                   "/Admin/Apps/Ecommerce/PayPalExpressCheckoutSettings", ACLOption.Create(new PayPalExpressCheckoutSettingsACL(), PayPalExpressCheckoutSettingsACL.View)),
                                               new ChildMenuItem("SagePay",
                                                   "/Admin/Apps/Ecommerce/SagePaySettings", ACLOption.Create(new SagePaySettingsACL(), SagePaySettingsACL.View)),
                                               new ChildMenuItem("WorldPay",
                                                   "/Admin/Apps/Ecommerce/WorldPaySettings", ACLOption.Create(new WorldPaySettingsACL(), WorldPaySettingsACL.View)),
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