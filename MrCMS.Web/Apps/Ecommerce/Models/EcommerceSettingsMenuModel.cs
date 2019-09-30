using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceSettingsMenuModel : IAdminMenuItem
    {
        private readonly IShippingMethodSubmenuGenerator _shippingMethodSubmenuGenerator;
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly UrlHelper _urlHelper;

        public EcommerceSettingsMenuModel(IShippingMethodSubmenuGenerator shippingMethodSubmenuGenerator,
            EcommerceSettings ecommerceSettings, UrlHelper urlHelper)
        {
            _shippingMethodSubmenuGenerator = shippingMethodSubmenuGenerator;
            _ecommerceSettings = ecommerceSettings;
            _urlHelper = urlHelper;
        }

        public string Text
        {
            get { return "Ecommerce Settings"; }
        }

        public string IconClass { get { return "fa fa-cogs"; } }

        public string Url { get; private set; }

        public bool CanShow
        {
            get { return CurrentRequestData.CurrentUser.CanAccess<ECommerceAdminMenuACL>(ECommerceAdminMenuACL.ShowMenu); }
        }

        public SubMenu Children
        {
            get
            {
                var subMenu = new SubMenu
                {
                    new ChildMenuItem("Global Settings", _urlHelper.Action("Edit", "EcommerceSettings"),
                        ACLOption.Create(new EcommerceSettingsACL(), EcommerceSettingsACL.Edit)),
                    new ChildMenuItem("Search Cache Settings", _urlHelper.Action("Edit", "EcommerceSearchCacheSettings"),
                        ACLOption.Create(new EcommerceSearchCacheSettingsACL(), EcommerceSearchCacheSettingsACL.Edit)),
                    new ChildMenuItem("Currencies", _urlHelper.Action("Index", "Currency"),
                        ACLOption.Create(new CurrencyACL(), CurrencyACL.List)),
                    new ChildMenuItem("Geographic Data", _urlHelper.Action("Index", "Country"),
                        ACLOption.Create(new CountryACL(), CountryACL.List)),
                    new ChildMenuItem("Taxes", _urlHelper.Action("Index", "TaxRate"),
                        ACLOption.Create(new TaxRateACL(), TaxRateACL.List))
                };

                if (_ecommerceSettings.RewardPointsEnabled)
                {
                    subMenu.Add(new ChildMenuItem("Reward Points", _urlHelper.Action("Index", "RewardPointSettings"),
                        ACLOption.Create(new RewardPointACL(), RewardPointACL.Settings)));
                }

                subMenu.Add(new ChildMenuItem("Shipping", "#", ACLOption.Create(new ShippingMethodACL(), ShippingMethodACL.List), _shippingMethodSubmenuGenerator.Get()));
                subMenu.Add(new ChildMenuItem("Payment Settings", "#", subMenu: new SubMenu
                {
                    new ChildMenuItem("Payment Settings",
                        _urlHelper.Action("Index", "PaymentSettings"),
                        ACLOption.Create(new PaymentSettingsACL(), PaymentSettingsACL.View)),

                    new ChildMenuItem("Paypoint", _urlHelper.Action("Index", "PaypointSettings"),
                        ACLOption.Create(new PaypointSettingsACL(), PaypointSettingsACL.View)),

                    new ChildMenuItem("PayPal Express Checkout", _urlHelper.Action("Index", "PayPalExpressCheckoutSettings"),
                        ACLOption.Create(new PayPalExpressCheckoutSettingsACL(), PayPalExpressCheckoutSettingsACL.View)),

                    new ChildMenuItem("SagePay", _urlHelper.Action("Index", "SagePaySettings"),
                        ACLOption.Create(new SagePaySettingsACL(), SagePaySettingsACL.View)),

                    new ChildMenuItem("WorldPay", _urlHelper.Action("Index", "WorldPaySettings"),
                        ACLOption.Create(new WorldPaySettingsACL(), WorldPaySettingsACL.View)),

                    new ChildMenuItem("Charity Clear", _urlHelper.Action("Index", "CharityClearSettings"),
                        ACLOption.Create(new WorldPaySettingsACL(), WorldPaySettingsACL.View)),

                    new ChildMenuItem("Braintree", _urlHelper.Action("Index", "BraintreeSettings"),
                        ACLOption.Create(new BraintreeSettingsACL(), BraintreeSettingsACL.View)),

                    new ChildMenuItem("Stripe", _urlHelper.Action("Index", "StripeSettings"),
                        ACLOption.Create(new StripeSettingsACL(), StripeSettingsACL.View)),

                    new ChildMenuItem("Elavon", _urlHelper.Action("Index", "ElavonSettings"),
                        ACLOption.Create(new ElavonSettingsACL(), ElavonSettingsACL.View))
                }));

                subMenu.Add(new ChildMenuItem("Product Review Settings", "/Admin/Apps/Ecommerce/ProductReviewSettings/Edit",
                        ACLOption.Create(new ProductReviewSettingsAcl(), ProductReviewSettingsAcl.Edit)));
                return subMenu;
            }
        }

        public int DisplayOrder
        {
            get { return 52; }
        }
    }
}