using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using Ninject;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce
{
    public class EcommerceApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Ecommerce"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {

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
            context.MapRoute("Product Search - Results", "Apps/Ecommerce/ProductSearch/Results", new { controller = "ProductSearch", action = "Results" });
            context.MapRoute("Category - Results", "Apps/Ecommerce/Category/Results", new { controller = "Category", action = "Results" });
            context.MapRoute("Cart - Details", "Apps/Ecommerce/Cart/Details", new { controller = "Cart", action = "Details" });
            context.MapRoute("Cart - Update Quantity", "Apps/Ecommerce/Cart/UpdateQuantity", new { controller = "Cart", action = "UpdateQuantity" });
            context.MapRoute("Cart - Basic Details", "Apps/Ecommerce/Cart/BasicDetails", new { controller = "Cart", action = "BasicDetails" });
            context.MapRoute("Cart - Delivery Details", "Apps/Ecommerce/Cart/DeliveryDetails", new { controller = "Cart", action = "DeliveryDetails" });
            context.MapRoute("Cart - Order Email", "Apps/Ecommerce/Cart/OrderEmail", new { controller = "Cart", action = "OrderEmail" });
            context.MapRoute("Cart - Enter Order Email", "Apps/Ecommerce/Cart/EnterOrderEmail", new { controller = "Cart", action = "EnterOrderEmail" });
            context.MapRoute("Cart - Order Placed", "Apps/Ecommerce/Cart/OrderPlaced", new { controller = "Cart", action = "OrderPlaced" });
            context.MapRoute("Cart - Payment Details", "Apps/Ecommerce/Cart/PaymentDetails", new { controller = "Cart", action = "PaymentDetails" });
            context.MapRoute("Cart - Set Delivery Details", "Apps/Ecommerce/Cart/SetDeliveryDetails", new { controller = "Cart", action = "SetDeliveryDetails" });
            context.MapRoute("Cart - Shipping Methods", "Apps/Ecommerce/Cart/ShippingMethods", new { controller = "Cart", action = "ShippingMethods" });
            context.MapRoute("Cart - Add Shipping Method", "Apps/Ecommerce/Cart/AddShippingMethod", new { controller = "Cart", action = "AddShippingMethod" });
            context.MapRoute("Cart - Get Shipping Calculation Country", "Apps/Ecommerce/Cart/GetShippingCalculationCountry", new { controller = "Cart", action = "GetShippingCalculationCountry" });
            context.MapRoute("Cart - Cart Panel", "Apps/Ecommerce/Cart/CartPanel", new { controller = "Cart", action = "CartPanel" });
            context.MapRoute("Cart - Add to Cart", "Apps/Ecommerce/Cart/AddToCart", new { controller = "Cart", action = "AddToCart" });
            context.MapRoute("Cart - Edit Cart Item", "Apps/Ecommerce/Cart/EditCartItem", new { controller = "Cart", action = "EditCartItem" });
            context.MapRoute("Cart - Delete Cart Item", "Apps/Ecommerce/Cart/DeleteCartItem", new { controller = "Cart", action = "DeleteCartItem" });
            context.MapRoute("Cart - Discount Code", "Apps/Ecommerce/Cart/DiscountCode", new { controller = "Cart", action = "DiscountCode" });
            context.MapRoute("Cart - Add Discount Code", "Apps/Ecommerce/Cart/AddDiscountCode", new { controller = "Cart", action = "AddDiscountCode" });
            context.MapRoute("Cart - Add Discount Code Ajax", "Apps/Ecommerce/Cart/AddDiscountCodeAjax", new { controller = "Cart", action = "AddDiscountCodeAjax" });
            context.MapRoute("Cart - Edit Discount Code", "Apps/Ecommerce/Cart/EditDiscountCode", new { controller = "Cart", action = "EditDiscountCode" });
            context.MapRoute("Cart - Is Discount Code Valid", "Apps/Ecommerce/Cart/IsDiscountCodeValid", new { controller = "Cart", action = "IsDiscountCodeValid" });
            context.MapRoute("User Login", "Apps/Ecommerce/UserLogin/UserLogin", new { controller = "UserLogin", action = "UserLogin" });
            context.MapRoute("User Login Details", "Apps/Ecommerce/UserLogin/UserLoginDetails", new { controller = "UserLogin", action = "UserLoginDetails" });
            context.MapRoute("User Login POST", "Apps/Ecommerce/UserLogin/Login", new { controller = "UserLogin", action = "Login" });
            context.MapRoute("User Registration", "Apps/Ecommerce/UserRegistration/UserRegistration", new { controller = "UserRegistration", action = "UserRegistration" });
            context.MapRoute("User Registration Details", "Apps/Ecommerce/UserRegistration/UserRegistrationDetails", new { controller = "UserRegistration", action = "UserRegistrationDetails" });
            context.MapRoute("User Register", "Apps/Ecommerce/UserRegistration/Register", new { controller = "UserRegistration", action = "Register" });
            context.MapRoute("User Account", "Apps/Ecommerce/UserAccount/UserAccount", new { controller = "UserAccount", action = "UserAccount" });
            context.MapRoute("User Account Details", "Apps/Ecommerce/UserAccount/UserAccountDetails", new { controller = "UserAccount", action = "UserAccountDetails" });
            context.MapRoute("User Account Orders", "Apps/Ecommerce/UserAccount/UserAccountOrders", new { controller = "UserAccount", action = "UserAccountOrders" });
            context.MapRoute("User Update Account", "Apps/Ecommerce/UserAccount/UpdateAccount", new { controller = "UserAccount", action = "UpdateAccount" });
            context.MapRoute("PayPal Express Checkout - SetExpressCheckout",
                             "Apps/Ecommerce/PayPalExpress/SetExpressCheckout",
                             new {controller = "PayPalExpressCheckout", action = "SetExpressCheckout"},
                             new[] {typeof (PayPalExpressCheckoutSettingsController).Namespace});
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            var currentSite = new CurrentSite(site);
            var configurationProvider = new ConfigurationProvider(new SettingService(session), currentSite);
            var siteSettings = configurationProvider.GetSiteSettings<SiteSettings>();
            var ecommerceSettings = configurationProvider.GetSiteSettings<EcommerceSettings>();
            var documentService = new DocumentService(session, siteSettings, currentSite);
            var widgetService = new WidgetService(session);
            var productSearch = new ProductSearch
                                    {
                                        Name = "Product Search Container",
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

            var layout = new Layout
                             {
                                 Name = "Ecommerce Layout",
                                 UrlSegment = "~/Themes/Ecommerce/Apps/Ecommerce/Views/Shared/_EcommerceLayout.cshtml",
                                 LayoutAreas = new List<LayoutArea>()
                             };
            var areas = new List<LayoutArea>
                                     {
                                         new LayoutArea {AreaName = "Logo", Layout = layout},
                                         new LayoutArea {AreaName = "Header", Layout = layout},
                                         new LayoutArea {AreaName = "After Content", Layout = layout},
                                         new LayoutArea {AreaName = "Before Content", Layout = layout},
                                         new LayoutArea {AreaName = "Footer", Layout = layout}
                                     };
            documentService.AddDocument(layout);
            var layoutAreaService = new LayoutAreaService(session);
            foreach (var area in areas)
                layoutAreaService.SaveArea(area);

            //widget setup footer links
            var footerLinksWidget = new TextWidget
                {
                    LayoutArea = areas.Single(x => x.AreaName == "Footer"),
                    Name = "Footer links",
                    Text = GetFooterLinksText()
                };
            widgetService.AddWidget(footerLinksWidget);
            

            siteSettings.DefaultLayoutId = layout.Id;
            siteSettings.ThemeName = "Ecommerce";
            configurationProvider.SaveSettings(siteSettings);
            ecommerceSettings.CategoryProductsPerPage = "12,20,40";
            ecommerceSettings.PageSizeAdmin = 20;
            configurationProvider.SaveSettings(ecommerceSettings);
            var checkoutLayout = new Layout
            {
                Name = "Checkout Layout",
                UrlSegment = "~/Themes/Ecommerce/Apps/Ecommerce/Views/Shared/_CheckoutLayout.cshtml",
                LayoutAreas = new List<LayoutArea>()
            };
            documentService.AddDocument(checkoutLayout);

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
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(enterOrderEmail);
            var setPaymentDetails = new PaymentDetails
            {
                Name = "Set Payment Details",
                UrlSegment = "set-payment-details",
                RevealInNavigation = true,
                Parent = yourBasket,
                DisplayOrder = 1,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(setPaymentDetails);
            var setDeliveryDetails = new SetDeliveryDetails
            {
                Name = "Set Delivery Details",
                UrlSegment = "set-delivery-details",
                RevealInNavigation = true,
                Parent = yourBasket,
                DisplayOrder = 2,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(setDeliveryDetails);
            var orderPlaced = new OrderPlaced
            {
                Name = "Order Placed",
                UrlSegment = "order-placed",
                RevealInNavigation = true,
                Parent = yourBasket,
                DisplayOrder = 3,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(orderPlaced);
            var myAccount = new UserAccount
            {
                Name = "My Account",
                UrlSegment = "my-account",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(myAccount);
            var login = new UserLogin
            {
                Name = "Login",
                UrlSegment = "log-in",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(login);
            var registration = new UserRegistration()
            {
                Name = "User Registration",
                UrlSegment = "register",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(registration);
            
        }

        private string GetFooterLinksText()
        {
            return @"<ul><li><a href=""#"">About &bull;</a></li><li><a href=""#"">Contact Us &bull;</a></li><li><a href=""#"">Privacy Policy &amp; Cookie Info &bull;</a></li><li><a href=""#"">Mr CMS</a></li></ul>";
        }
    }
}