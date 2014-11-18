using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceSettingsMenuModel : IAdminMenuItem
    {
        private readonly IShippingMethodSubmenuGenerator _shippingMethodSubmenuGenerator;
        private readonly EcommerceSettings _ecommerceSettings;

        public EcommerceSettingsMenuModel(IShippingMethodSubmenuGenerator shippingMethodSubmenuGenerator,
            EcommerceSettings ecommerceSettings)
        {
            _shippingMethodSubmenuGenerator = shippingMethodSubmenuGenerator;
            _ecommerceSettings = ecommerceSettings;
        }

        public string Text
        {
            get { return "Settings"; }
        }

        public string IconClass { get { return "fa fa-cogs"; } }

        public string Url { get; private set; }

        public bool CanShow
        {
            get { return CurrentRequestData.CurrentUser.CanAccess<CatalogACL>(CatalogACL.Show); }
        }

        public SubMenu Children
        {
            get
            {
                return new SubMenu
                {
                    new ChildMenuItem("Global Settings", "/Admin/Apps/Ecommerce/EcommerceSettings/Edit",
                        ACLOption.Create(new EcommerceSettingsACL(), EcommerceSettingsACL.Edit)),
                    new ChildMenuItem("Search Cache Settings", "/Admin/Apps/Ecommerce/EcommerceSearchCacheSettings/Edit",
                        ACLOption.Create(new EcommerceSearchCacheSettingsACL(), EcommerceSearchCacheSettingsACL.Edit)),
                    new ChildMenuItem("Currencies", "/Admin/Apps/Ecommerce/Currency",
                        ACLOption.Create(new CurrencyACL(), CurrencyACL.List)),
                    new ChildMenuItem("Geographic Data", "/Admin/Apps/Ecommerce/Country",
                        ACLOption.Create(new CountryACL(), CountryACL.List)),
                    new ChildMenuItem("Taxes", "/Admin/Apps/Ecommerce/TaxRate",
                        ACLOption.Create(new TaxRateACL(), TaxRateACL.List)),
                    new ChildMenuItem("Shipping", "#", ACLOption.Create(new ShippingMethodACL(), ShippingMethodACL.List),
                        subMenu: _shippingMethodSubmenuGenerator.Get()),
                    new ChildMenuItem("Payment Settings", "#", subMenu: new SubMenu
                    {
                        new ChildMenuItem("Payment Settings",
                            "/Admin/Apps/Ecommerce/PaymentSettings",  ACLOption.Create(new PaymentSettingsACL(), PaymentSettingsACL.View)),
                        new ChildMenuItem("Paypoint", "/Admin/Apps/Ecommerce/PaypointSettings",  ACLOption.Create(new PaypointSettingsACL(), PaypointSettingsACL.View)),
                        new ChildMenuItem("PayPal Express Checkout", "/Admin/Apps/Ecommerce/PayPalExpressCheckoutSettings", ACLOption.Create(new PayPalExpressCheckoutSettingsACL(), PayPalExpressCheckoutSettingsACL.View)),
                        new ChildMenuItem("SagePay", "/Admin/Apps/Ecommerce/SagePaySettings", ACLOption.Create(new SagePaySettingsACL(), SagePaySettingsACL.View)),
                        new ChildMenuItem("WorldPay","/Admin/Apps/Ecommerce/WorldPaySettings", ACLOption.Create(new WorldPaySettingsACL(), WorldPaySettingsACL.View)),                      
                        new ChildMenuItem("Charity Clear","/Admin/Apps/Ecommerce/CharityClearSettings", ACLOption.Create(new WorldPaySettingsACL(), WorldPaySettingsACL.View)),                      

                    }),
                    new ChildMenuItem("Product Review Settings", "/Admin/Apps/Ecommerce/ProductReviewSettings/Edit",
                        ACLOption.Create(new ProductReviewSettingsAcl(), ProductReviewSettingsAcl.Edit)),
                };
            }
        }

        public int DisplayOrder
        {
            get { return 52; }
        }
    }
}