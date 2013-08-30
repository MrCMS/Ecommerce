using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.PaypointService.API;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using NHibernate;
using Ninject;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Ninject.Web.Common;

namespace MrCMS.Web.Apps.Ecommerce
{
    public class EcommerceApp : MrCMSApp
    {
        public const string EcommerceAppName = "Ecommerce";

        public override string AppName
        {
            get { return EcommerceAppName; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
            kernel.Rebind<CartModel>().ToMethod(context => context.Kernel.Get<ICartBuilder>().BuildCart()).InRequestScope();
            kernel.Bind<SECVPN>().To<SECVPNClient>().InRequestScope();
        }

        public override IEnumerable<Type> BaseTypes
        {
            get
            {
                yield return typeof(DiscountLimitation);
                yield return typeof(DiscountApplication);
            }
        }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapAreaRoute("Admin controllers", "Admin", "Admin/Apps/Ecommerce/{controller}/{action}/{id}",
                                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                                 new[] { typeof(ProductController).Namespace });
            context.MapRoute("Product Variant - GetPriceBreaksForProductVariant", "Apps/Ecommerce/ProductVariant/GetPriceBreaksForProductVariant", new { controller = "ProductVariant", action = "GetPriceBreaksForProductVariant" });
            context.MapRoute("Cart - Details", "Apps/Ecommerce/CartDetails", new { controller = "Cart", action = "Details" });
            context.MapRoute("Cart - Update Basket", "Apps/Ecommerce/UpdateBasket", new { controller = "Cart", action = "UpdateBasket" });
            context.MapRoute("Cart - Empty Basket", "Apps/Ecommerce/EmptyBasket", new { controller = "Cart", action = "EmptyBasket" });
            context.MapRoute("Cart - Add to Cart", "Apps/Ecommerce/AddToCart", new { controller = "Cart", action = "AddToCart" });
            context.MapRoute("Cart - Edit Cart Item", "Apps/Ecommerce/EditCartItem", new { controller = "Cart", action = "EditCartItem" });
            context.MapRoute("Cart - Delete Cart Item", "Apps/Ecommerce/DeleteCartItem", new { controller = "Cart", action = "DeleteCartItem" });
            context.MapRoute("Cart - Apply Discount Code", "Apps/Ecommerce/ApplyDiscountCode", new { controller = "Cart", action = "ApplyDiscountCode" });

            context.MapRoute("Set Delivery Details - Get Delivery Address",
                             "Apps/Ecommerce/SetDeliveryDetails/DeliveryAddress",
                             new { controller = "SetDeliveryDetails", action = "DeliveryAddress" },
                             new[] { typeof(SetDeliveryDetailsController).Namespace });

            context.MapRoute("Set Delivery Details - Set Shipping Method",
                              "Apps/Ecommerce/SetDeliveryDetails/SetShipping",
                              new { controller = "SetDeliveryDetails", action = "SetShipping" },
                              new[] { typeof(SetDeliveryDetailsController).Namespace });

            context.MapRoute("Set Delivery Details - Set Shipping Address",
                              "Apps/Ecommerce/SetDeliveryDetails/SetAddress",
                              new { controller = "SetDeliveryDetails", action = "SetAddress" },
                              new[] { typeof(SetDeliveryDetailsController).Namespace });

            context.MapRoute("Enter Order Email - Set Order Email",
                             "Apps/Ecommerce/SetOrderEmail",
                             new { controller = "EnterOrderEmail", action = "SetOrderEmail" },
                             new[] { typeof(EnterOrderEmailController).Namespace });

            context.MapRoute("Enter Order Email - Set Order Email And Login",
                                        "Apps/Ecommerce/SetOrderEmailAndLogin",
                                        new { controller = "EnterOrderEmail", action = "SetOrderEmailAndLogin" },
                                        new[] { typeof(EnterOrderEmailController).Namespace });

            context.MapRoute("Checkout - Price Summary",
                             "Apps/Ecommerce/Checkout/Summary",
                             new { controller = "Checkout", action = "Summary" },
                             new[] { typeof(CheckoutController).Namespace });

            context.MapRoute("Confirm Order - Cash On Delivery",
                             "Apps/Ecommerce/Confirm/CashOnDelivery",
                             new { controller = "PaymentMethod", action = "CashOnDelivery" },
                             new[] { typeof(PaymentMethodController).Namespace });

            context.MapRoute("Confirm Order - PayPal Express Checkout",
                             "Apps/Ecommerce/Confirm/PayPalExpressCheckout",
                             new { controller = "PaymentMethod", action = "PayPalExpressCheckout" },
                             new[] { typeof(PaymentMethodController).Namespace });

            context.MapRoute("Confirm Order - Paypoint",
                             "Apps/Ecommerce/Confirm/Paypoint",
                             new { controller = "PaymentMethod", action = "Paypoint" },
                             new[] { typeof(PaymentMethodController).Namespace });

            context.MapRoute("Checkout - Billing Address same as Shipping Address",
                             "Apps/Ecommerce/PaymentDetails/BillingAddressSameAsShippingAddress",
                             new { controller = "PaymentDetails", action = "BillingAddressSameAsShippingAddress" },
                             new[] { typeof(PaymentDetailsController).Namespace });

            context.MapRoute("Checkout - Update Billing Address",
                             "Apps/Ecommerce/PaymentDetails/UpdateBillingAddress",
                             new { controller = "PaymentDetails", action = "UpdateBillingAddress" },
                             new[] { typeof(PaymentDetailsController).Namespace });

            context.MapRoute("Checkout - Save Billing Address",
                             "Apps/Ecommerce/PaymentDetails/SaveBillingAddress",
                             new { controller = "PaymentDetails", action = "SaveBillingAddress" },
                             new[] { typeof(PaymentDetailsController).Namespace });

            context.MapRoute("User Account Orders", "Apps/Ecommerce/UserAccount/UserAccountOrders", new { controller = "UserAccount", action = "UserAccountOrders" });
            context.MapRoute("User Account Register Without Details", "Apps/Ecommerce/UserAccount/RegistrationWithoutDetails", new { controller = "UserAccount", action = "RegistrationWithoutDetails" });

            context.MapRoute("PayPal Express Checkout - SetExpressCheckout",
                             "Apps/Ecommerce/PayPalExpress/SetExpressCheckout",
                             new { controller = "PayPalExpressCheckout", action = "SetExpressCheckout" },
                             new[] { typeof(PayPalExpressCheckoutController).Namespace });
            context.MapRoute("Product Search - Query",
                             "search/query",
                             new { controller = "ProductSearch", action = "Query" },
                             new[] { typeof(ProductSearchController).Namespace });
            context.MapRoute("Product Search - Results",
                             "search/results",
                             new { controller = "ProductSearch", action = "Results" },
                             new[] { typeof(ProductSearchController).Namespace });

            context.MapRoute("Checkout - PayPal Return Handler",
                             "Apps/Ecommerce/PayPalExpressCheckout/ReturnHandler",
                             new { controller = "PayPalExpressCheckout", action = "Return" },
                             new[] { typeof(PayPalExpressCheckoutController).Namespace });

            context.MapRoute("Checkout - Paypoint 3D Secure Redirect",
                             "Apps/Ecommerce/Paypoint/3DSecureRedirect",
                             new { controller = "Paypoint", action = "Redirect3DSecure" },
                             new[] { typeof(PaypointController).Namespace });

            context.MapRoute("Checkout - Paypoint 3D Secure Response Handler",
                             "Apps/Ecommerce/Paypoint/3DSecureReturnHandler",
                             new { controller = "Paypoint", action = "Response3DSecure" },
                             new[] { typeof(PaypointController).Namespace });
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            var configurationProvider = new ConfigurationProvider(new SettingService(session), site);
            var siteSettings = configurationProvider.GetSiteSettings<SiteSettings>();
            var ecommerceSettings = configurationProvider.GetSiteSettings<EcommerceSettings>();
            var documentService = new DocumentService(session, siteSettings, site);
            var currencyService = new CurrencyService(session);

            var widgetService = new WidgetService(session);
            var productSearch = new ProductSearch
                                    {
                                        Name = "Products",
                                        UrlSegment = "products",
                                        RevealInNavigation = true
                                    };
            var categoryContainer = new CategoryContainer
                                        {
                                            Name = "Categories",
                                            UrlSegment = "categories",
                                            RevealInNavigation = true
                                        };
            documentService.AddDocument(productSearch);
            documentService.PublishNow(productSearch);
            documentService.AddDocument(categoryContainer);
            documentService.PublishNow(categoryContainer);

            //base layout
            var baseLayout = new Layout
            {
                Name = "Base Ecommerce Layout",
                UrlSegment = "~/Apps/Ecommerce/Views/Shared/_BaseLayout.cshtml",
                LayoutAreas = new List<LayoutArea>()
            };
            documentService.AddDocument(baseLayout);
            //ecommerce main layout
            var eCommerceLayout = new Layout
                             {
                                 Name = "Ecommerce Layout",
                                 UrlSegment = "~/Apps/Ecommerce/Views/Shared/_EcommerceLayout.cshtml",
                                 LayoutAreas = new List<LayoutArea>(),
                                 Parent = baseLayout
                             };
            var ecommerceLayoutArea = new List<LayoutArea>
                                     {
                                         new LayoutArea {AreaName = "Header left", Layout = eCommerceLayout},
                                         new LayoutArea {AreaName = "Header Middle", Layout = eCommerceLayout},
                                         new LayoutArea {AreaName = "Header Right", Layout = eCommerceLayout},
                                         new LayoutArea {AreaName = "Navigation", Layout = eCommerceLayout},
                                         new LayoutArea {AreaName = "After Content", Layout = eCommerceLayout},
                                         new LayoutArea {AreaName = "Before Content", Layout = eCommerceLayout},
                                         new LayoutArea {AreaName = "Footer", Layout = eCommerceLayout}
                                     };
            documentService.AddDocument(eCommerceLayout);
            var layoutAreaService = new LayoutAreaService(session);
            foreach (var area in ecommerceLayoutArea)
                layoutAreaService.SaveArea(area);
            //checkout layout
            var checkoutLayout = new Layout
            {
                Name = "Checkout Layout",
                UrlSegment = "~/Apps/Ecommerce/Views/Shared/_CheckoutLayout.cshtml",
                LayoutAreas = new List<LayoutArea>(),
                Parent = eCommerceLayout
            };
            documentService.AddDocument(checkoutLayout);
            //product layout
            var productLayout = new Layout
            {
                Name = "Product Layout",
                UrlSegment = "~/Apps/Ecommerce/Views/Shared/_ProductLayout.cshtml",
                LayoutAreas = new List<LayoutArea>(),
                Parent = eCommerceLayout
            };
            var productLayoutAreas = new List<LayoutArea>
                                     {
                                         new LayoutArea {AreaName = "Below Product Price", Layout = productLayout},
                                         new LayoutArea {AreaName = "Below Add to cart", Layout = productLayout}
                                     };

            documentService.AddDocument(productLayout);
            foreach (var area in productLayoutAreas)
                layoutAreaService.SaveArea(area);

            //category/search layout
            var searchLayout = new Layout
            {
                Name = "Search Layout",
                UrlSegment = "~/Apps/Ecommerce/Views/Shared/_SearchLayout.cshtml",
                LayoutAreas = new List<LayoutArea>(),
                Parent = eCommerceLayout
            };
            var searchLayoutAreas = new List<LayoutArea>
                                     {
                                         new LayoutArea {AreaName = "Before filters", Layout = searchLayout},
                                         new LayoutArea {AreaName = "After filters", Layout = searchLayout}
                                     };

            documentService.AddDocument(searchLayout);
            foreach (var area in searchLayoutAreas)
                layoutAreaService.SaveArea(area);

            //widget setup footer links
            var footerLinksWidget = new TextWidget
                {
                    LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "Footer"),
                    Name = "Footer links",
                    Text = GetFooterLinksText()
                };
            widgetService.AddWidget(footerLinksWidget);


            siteSettings.DefaultLayoutId = eCommerceLayout.Id;
            siteSettings.ThemeName = "Ecommerce";
            configurationProvider.SaveSettings(siteSettings);
            ecommerceSettings.SearchProductsPerPage = "12,20,40";
            ecommerceSettings.PageSizeAdmin = 20;
            ecommerceSettings.PreviousPriceText = "Previous price";

            configurationProvider.SaveSettings(ecommerceSettings);


            var welcome = new TextPage
            {
                Name = "Welcome",
                UrlSegment = "shop",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(welcome);
            var yourBasket = new Cart
            {
                Name = "Your Basket",
                UrlSegment = "basket",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(yourBasket);
            var enterOrderEmail = new EnterOrderEmail
            {
                Name = "Enter Order Email",
                UrlSegment = "enter-order-email",
                RevealInNavigation = true,
                Parent = yourBasket,
                DisplayOrder = 0,
                PublishOn = DateTime.UtcNow,
                Layout = checkoutLayout
            };
            documentService.AddDocument(enterOrderEmail);
            var setPaymentDetails = new PaymentDetails
            {
                Name = "Set Payment Details",
                UrlSegment = "set-payment-details",
                RevealInNavigation = true,
                Parent = yourBasket,
                DisplayOrder = 1,
                PublishOn = DateTime.UtcNow,
                Layout = checkoutLayout
            };
            documentService.AddDocument(setPaymentDetails);
            var setDeliveryDetails = new SetDeliveryDetails
            {
                Name = "Set Delivery Details",
                UrlSegment = "set-delivery-details",
                RevealInNavigation = true,
                Parent = yourBasket,
                DisplayOrder = 2,
                PublishOn = DateTime.UtcNow,
                Layout = checkoutLayout
            };
            documentService.AddDocument(setDeliveryDetails);
            var orderPlaced = new OrderPlaced
            {
                Name = "Order Placed",
                UrlSegment = "order-placed",
                RevealInNavigation = true,
                Parent = yourBasket,
                DisplayOrder = 3,
                PublishOn = DateTime.UtcNow,
                Layout = checkoutLayout
            };
            documentService.AddDocument(orderPlaced);
            
            //add currency
            var britishCurrency = new MrCMS.Web.Apps.Ecommerce.Entities.Currencies.Currency
                {
                    Name = "British Pound",
                    Code = "GBP",
                    Format = "£0.00"
                };
            currencyService.Add(britishCurrency);

        }

        private string GetFooterLinksText()
        {
            return @"<ul><li><a href=""#"">About &bull;</a></li><li><a href=""#"">Contact Us &bull;</a></li><li><a href=""#"">Privacy Policy &amp; Cookie Info &bull;</a></li><li><a href=""#"">Mr CMS</a></li></ul>";
        }

        public override IEnumerable<System.Type> Conventions
        {
            get { yield return typeof(TableNameConvention); }
        }
    }
}