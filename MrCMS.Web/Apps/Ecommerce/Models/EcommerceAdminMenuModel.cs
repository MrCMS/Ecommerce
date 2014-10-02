using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceAdminMenuModel : IAdminMenuItem
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly UrlHelper _urlHelper;
        private readonly IShippingMethodSubmenuGenerator _shippingMethodSubmenuGenerator;

        public EcommerceAdminMenuModel(IShippingMethodSubmenuGenerator shippingMethodSubmenuGenerator,
            EcommerceSettings ecommerceSettings, UrlHelper urlHelper)
        {
            _shippingMethodSubmenuGenerator = shippingMethodSubmenuGenerator;
            _ecommerceSettings = ecommerceSettings;
            _urlHelper = urlHelper;
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
                    new ChildMenuItem("Orders", _urlHelper.Action("Index", "Order"),
                        ACLOption.Create(new OrderACL(), OrderACL.List)),
                    new ChildMenuItem("Products", _urlHelper.Action("Index", "Product"),
                        ACLOption.Create(new ProductACL(), ProductACL.List)),
                    new ChildMenuItem("Categories", _urlHelper.Action("Index", "Category"),
                        ACLOption.Create(new CategoryACL(), CategoryACL.List)),
                    new ChildMenuItem("Brands", _urlHelper.Action("Index", "Brand"),
                        ACLOption.Create(new BrandACL(), BrandACL.List)),
                    new ChildMenuItem("Product Specifications",
                        _urlHelper.Action("Index", "ProductSpecificationAttribute"),
                        ACLOption.Create(new ProductSpecificationAttributeACL(), ProductSpecificationAttributeACL.List)),
                    new ChildMenuItem("Discounts", _urlHelper.Action("Index", "Discount"),
                        ACLOption.Create(new DiscountACL(), DiscountACL.List)),
                };
                if (_ecommerceSettings.GiftCardsEnabled)
                {
                    adminItems.Add(new ChildMenuItem("Gift Cards", _urlHelper.Action("Index", "GiftCard"),
                        ACLOption.Create(new GiftCardACL(), GiftCardACL.List)));
                }
                if (_ecommerceSettings.WarehouseStockEnabled)
                {
                    adminItems.Add(new ChildMenuItem("Warehouses", _urlHelper.Action("Index", "Warehouse"),
                        ACLOption.Create(new WarehouseACL(), WarehouseACL.List)));
                    adminItems.Add(new ChildMenuItem("Warehouse Stock", _urlHelper.Action("Index", "WarehouseStock"),
                        ACLOption.Create(new WarehouseStockACL(), WarehouseStockACL.List)));
                }
                var generalSettingsItems = new List<ChildMenuItem>
                {
                    new ChildMenuItem("Global Settings",
                        _urlHelper.Action("Edit", "EcommerceSettings"),
                        ACLOption.Create(new EcommerceSettingsACL(), EcommerceSettingsACL.Edit)),
                    new ChildMenuItem("Search Cache Settings",
                        _urlHelper.Action("Edit", "EcommerceSearchCacheSettings"),
                        ACLOption.Create(new EcommerceSearchCacheSettingsACL(),
                            EcommerceSearchCacheSettingsACL.Edit)),
                    new ChildMenuItem("Currencies",
                        _urlHelper.Action("Index", "Currency"),
                        ACLOption.Create(new CurrencyACL(), CurrencyACL.List)),
                    new ChildMenuItem("Geographic Data",
                        _urlHelper.Action("Index", "Country"),
                        ACLOption.Create(new CountryACL(), CountryACL.List)),
                    new ChildMenuItem("Taxes", _urlHelper.Action("Index", "TaxRate"),
                        ACLOption.Create(new TaxRateACL(), TaxRateACL.List))
                };
                var shippingSubMenu = _shippingMethodSubmenuGenerator.Get();
                if (shippingSubMenu.Any())
                {
                    generalSettingsItems.Add(new ChildMenuItem("Shipping", "#",
                        ACLOption.Create(new ShippingMethodACL(), ShippingMethodACL.List),
                        shippingSubMenu));
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
                            new ChildMenuItem("Templates",  _urlHelper.Action("Index", "NewsletterTemplate"),
                                ACLOption.Create(new NewsletterTemplateACL(), NewsletterTemplateACL.List)),
                            new ChildMenuItem("Newsletter",  _urlHelper.Action("Index", "Newsletter"),
                                ACLOption.Create(new NewsletterACL(), NewsletterACL.List))
                        }
                    },
                    {
                        "Settings",
                        new List<ChildMenuItem>
                        {
                            new ChildMenuItem("General Settings", "#", subMenu: new SubMenu
                            {
                                {
                                    "", generalSettingsItems
                                }
                            }),
                            new ChildMenuItem("Payment Settings", "#", subMenu: new SubMenu
                            {
                                {
                                    "", new List<ChildMenuItem>
                                    {
                                        new ChildMenuItem("Payment Settings",
                                             _urlHelper.Action("Index", "PaymentSettings"),
                                            ACLOption.Create(new PaymentSettingsACL(), PaymentSettingsACL.View)),
                                        new ChildMenuItem("Paypoint",
                                             _urlHelper.Action("Index", "PaypointSettings"),
                                            ACLOption.Create(new PaypointSettingsACL(), PaypointSettingsACL.View)),
                                        new ChildMenuItem("PayPal Express Checkout",
                                             _urlHelper.Action("Index", "PayPalExpressCheckoutSettings"),
                                            ACLOption.Create(new PayPalExpressCheckoutSettingsACL(),
                                                PayPalExpressCheckoutSettingsACL.View)),
                                        new ChildMenuItem("SagePay",
                                             _urlHelper.Action("Index", "SagePaySettings"),
                                            ACLOption.Create(new SagePaySettingsACL(), SagePaySettingsACL.View)),
                                        new ChildMenuItem("WorldPay",
                                             _urlHelper.Action("Index", "WorldPaySettings"),
                                            ACLOption.Create(new WorldPaySettingsACL(), WorldPaySettingsACL.View)),
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