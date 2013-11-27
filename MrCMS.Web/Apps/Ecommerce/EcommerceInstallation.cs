using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce
{
    public static class EcommerceInstallation
    {
        public static void InstallApp(ISession session, InstallModel model, Site site)
        {
            var configurationProvider = new ConfigurationProvider(new SettingService(session, site), site);
            var siteSettings = configurationProvider.GetSiteSettings<SiteSettings>();
            var ecommerceSettings = configurationProvider.GetSiteSettings<EcommerceSettings>();
            var eventService = MrCMSApplication.Get<IDocumentEventService>();
            var documentService = new DocumentService(session, eventService, siteSettings, site);
            var currencyService = new CurrencyService(session);
            var widgetService = new WidgetService(session);
            var defaultMediaCategory = documentService.GetDocumentByUrl<MediaCategory>("default");

            // file
            var fileSystem = new FileSystem();
            var mediaSettings = configurationProvider.GetSiteSettings<MediaSettings>();
            var imageProcessor = new ImageProcessor(session, fileSystem, mediaSettings);
            var fileService = new FileService(session, fileSystem, imageProcessor, mediaSettings, site, siteSettings);


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
                                              new LayoutArea {AreaName = "Header Left", Layout = eCommerceLayout},
                                              new LayoutArea {AreaName = "Header Middle", Layout = eCommerceLayout},
                                              new LayoutArea {AreaName = "Header Right", Layout = eCommerceLayout},
                                              new LayoutArea {AreaName = "User Links", Layout = eCommerceLayout},
                                              new LayoutArea {AreaName = "Navigation", Layout = eCommerceLayout},
                                              new LayoutArea {AreaName = "Before Content", Layout = eCommerceLayout},
                                              new LayoutArea {AreaName = "After Content", Layout = eCommerceLayout},
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
            var checkoutLayoutAreas = new List<LayoutArea>
                {
                    new LayoutArea {AreaName = "Header Left", Layout = checkoutLayout},
                    new LayoutArea {AreaName = "Header Right", Layout = checkoutLayout},
                    new LayoutArea {AreaName = "Footer Left", Layout = checkoutLayout},
                    new LayoutArea {AreaName = "Footer Right", Layout = checkoutLayout}
                };
            documentService.AddDocument(checkoutLayout);
            foreach (var area in checkoutLayoutAreas)
                layoutAreaService.SaveArea(area);

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
                                             new LayoutArea {AreaName = "Below Add to cart", Layout = productLayout},
                                             new LayoutArea {AreaName = "Below Product Information", Layout = productLayout}
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
                                            new LayoutArea {AreaName = "Before Filters", Layout = searchLayout},
                                            new LayoutArea {AreaName = "After Filters", Layout = searchLayout}
                                        };

            documentService.AddDocument(searchLayout);
            foreach (var area in searchLayoutAreas)
                layoutAreaService.SaveArea(area);

            //widgets for main layout

            //linked logo
            var mainLogoPath = HttpContext.Current.Server.MapPath("/Apps/Core/Content/images/mrcms-logo.png");
            var stream = new FileStream(mainLogoPath, FileMode.Open);
            var logoFile = fileService.AddFile(stream, Path.GetFileName(mainLogoPath), "image/png", stream.Length, defaultMediaCategory);
            var linkedImageLogo = new LinkedImage
            {
                Name = "Logo",
                Link = "/",
                Image = logoFile.url,
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "Header Left")
            };
            widgetService.AddWidget(linkedImageLogo);
            // Search
            var searchBox = new SearchBox
            {
                Name = "Search",
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "Header Middle")
            };
            widgetService.AddWidget(searchBox);

            //userlink
            var userLinks = new UserLinks
            {
                Name = "User Links",
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "User Links")
            };
            widgetService.AddWidget(userLinks);

            //cart widget
            var cartWidget = new CartWidget
            {
                Name = "Cart Widget",
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "Header Right")
            };
            widgetService.AddWidget(cartWidget);

            //nav
            var nav = new EcommerceNavigation
            {
                Name = "Navigation",
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == ("Navigation"))
            };
            widgetService.AddWidget(nav);

            //footer links
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
            ecommerceSettings.PreviousPriceText = "Previous price";


            
            var imgPath = HttpContext.Current.Server.MapPath("/Apps/Ecommerce/Content/Images/awaiting-image.jpg");
            var fileStream = new FileStream(imgPath, FileMode.Open);
            var dbFile = fileService.AddFile(fileStream, Path.GetFileName(imgPath), "image/jpeg", fileStream.Length, defaultMediaCategory);

            ecommerceSettings.DefaultNoProductImage = dbFile.url;

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
                RevealInNavigation = false,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(yourBasket);
            var enterOrderEmail = new EnterOrderEmail
            {
                Name = "Enter Order Email",
                UrlSegment = "enter-order-email",
                RevealInNavigation = false,
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
                RevealInNavigation = false,
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
                RevealInNavigation = false,
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
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 3,
                PublishOn = DateTime.UtcNow,
                Layout = checkoutLayout
            };
            documentService.AddDocument(orderPlaced);

            //Added to cart
            var addedToCart = new ProductAddedToCart()
            {
                Name = "Added to Basket",
                UrlSegment = "add-to-basket",
                RevealInNavigation = false,
                PublishOn = DateTime.UtcNow
            };
            documentService.AddDocument(addedToCart);

            //add currency
            var britishCurrency = new Entities.Currencies.Currency
                                      {
                                          Name = "British Pound",
                                          Code = "GBP",
                                          Format = "£0.00"
                                      };
            currencyService.Add(britishCurrency);
        }

        private static string GetFooterLinksText()
        {
            return @"<ul class=""inline""><li><a href=""#"">About &bull;</a></li><li><a href=""#"">Contact Us &bull;</a></li><li><a href=""#"">Privacy Policy &amp; Cookie Info &bull;</a></li><li><a href=""#"">Mr CMS</a></li></ul>";
        }
    }
}