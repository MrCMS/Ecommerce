using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Installation;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce
{
    public class EcommerceAppInstallation : IOnInstallation
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IDocumentService _documentService;
        private readonly ICurrencyService _currencyService;
        private readonly IWidgetService _widgetService;
        private readonly IFileService _fileService;
        private readonly ILayoutAreaAdminService _layoutAreaAdminService;

        public EcommerceAppInstallation(IConfigurationProvider configurationProvider, 
            IDocumentService documentService, ICurrencyService currencyService, IWidgetService widgetService, 
            IFileService fileService, ILayoutAreaAdminService layoutAreaAdminService)
        {
            _configurationProvider = configurationProvider;
            _documentService = documentService;
            _currencyService = currencyService;
            _widgetService = widgetService;
            _fileService = fileService;
            _layoutAreaAdminService = layoutAreaAdminService;
        }


        public int Priority
        {
            get { return -1; }
        }

        public void Install(InstallModel model)
        {
            var defaultMediaCategory = _documentService.GetDocumentByUrl<MediaCategory>("default");

            var productSearch = new ProductSearch
            {
                Name = "Products",
                UrlSegment = "products",
                RevealInNavigation = true
            };
            var categoryContainer = new ProductContainer
            {
                Name = "Categories",
                UrlSegment = "categories",
                RevealInNavigation = true
            };
            _documentService.AddDocument(productSearch);
            _documentService.PublishNow(productSearch);
            _documentService.AddDocument(categoryContainer);
            _documentService.PublishNow(categoryContainer);

            //base layout
            var baseLayout = new Layout
            {
                Name = "Base Ecommerce Layout",
                UrlSegment = "~/Apps/Ecommerce/Views/Shared/_BaseLayout.cshtml",
                LayoutAreas = new List<LayoutArea>()
            };
            _documentService.AddDocument(baseLayout);
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
            _documentService.AddDocument(eCommerceLayout);
            foreach (var area in ecommerceLayoutArea)
                _layoutAreaAdminService.SaveArea(area);
            //checkout layout
            var checkoutLayout = new Layout
            {
                Name = "Checkout Layout",
                UrlSegment = "_CheckoutLayout",
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
            _documentService.AddDocument(checkoutLayout);

            foreach (var area in checkoutLayoutAreas)
                _layoutAreaAdminService.SaveArea(area);

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

            _documentService.AddDocument(productLayout);
            foreach (var area in productLayoutAreas)
                _layoutAreaAdminService.SaveArea(area);

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

            _documentService.AddDocument(searchLayout);
            foreach (var area in searchLayoutAreas)
                _layoutAreaAdminService.SaveArea(area);

            //widgets for main layout

            //linked logo
            var mainLogoPath = HttpContext.Current.Server.MapPath("/Apps/Core/Content/images/mrcms-logo.png");
            var stream = new FileStream(mainLogoPath, FileMode.Open);
            var logoFile = _fileService.AddFile(stream, Path.GetFileName(mainLogoPath), "image/png", stream.Length, defaultMediaCategory);
            var linkedImageLogo = new LinkedImage
            {
                Name = "Logo",
                Link = "/",
                Image = logoFile.FileUrl,
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "Header Left")
            };
            _widgetService.AddWidget(linkedImageLogo);
            // Search
            var searchBox = new SearchBox
            {
                Name = "Search",
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "Header Middle")
            };
            _widgetService.AddWidget(searchBox);

            //userlink
            var userLinks = new UserLinks
            {
                Name = "User Links",
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "User Links")
            };
            _widgetService.AddWidget(userLinks);

            //cart widget
            var cartWidget = new CartWidget
            {
                Name = "Cart Widget",
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "Header Right")
            };
            _widgetService.AddWidget(cartWidget);

            //nav
            var nav = new EcommerceNavigation
            {
                Name = "Navigation",
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == ("Navigation"))
            };
            _widgetService.AddWidget(nav);

            //footer links
            var footerLinksWidget = new TextWidget
            {
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "Footer"),
                Name = "Footer links",
                Text = GetFooterLinksText()
            };
            _widgetService.AddWidget(footerLinksWidget);

            var siteSettings = _configurationProvider.GetSiteSettings<SiteSettings>();
            siteSettings.DefaultLayoutId = eCommerceLayout.Id;
            siteSettings.ThemeName = "Ecommerce";
            _configurationProvider.SaveSettings(siteSettings);
            var ecommerceSettings = _configurationProvider.GetSiteSettings<EcommerceSettings>();
            ecommerceSettings.SearchProductsPerPage = "12,20,40";
            ecommerceSettings.PreviousPriceText = "Previous price";



            var imgPath = HttpContext.Current.Server.MapPath("/Apps/Ecommerce/Content/Images/awaiting-image.jpg");
            var fileStream = new FileStream(imgPath, FileMode.Open);
            var dbFile = _fileService.AddFile(fileStream, Path.GetFileName(imgPath), "image/jpeg", fileStream.Length, defaultMediaCategory);

            ecommerceSettings.DefaultNoProductImage = dbFile.FileUrl;

            _configurationProvider.SaveSettings(ecommerceSettings);


            var welcome = new TextPage
            {
                Name = "Welcome",
                UrlSegment = "shop",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow
            };
            _documentService.AddDocument(welcome);
            var yourBasket = new Cart
            {
                Name = "Your Basket",
                UrlSegment = "basket",
                RevealInNavigation = false,
                PublishOn = DateTime.UtcNow
            };
            _documentService.AddDocument(yourBasket);
            var enterOrderEmail = new EnterOrderEmail
            {
                Name = "Enter Order Email",
                UrlSegment = "enter-order-email",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 0,
                PublishOn = DateTime.UtcNow,
            };
            _documentService.AddDocument(enterOrderEmail);
            var setPaymentDetails = new PaymentDetails
            {
                Name = "Set Payment Details",
                UrlSegment = "set-payment-details",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 1,
                PublishOn = DateTime.UtcNow,
            };
            _documentService.AddDocument(setPaymentDetails);
            var setDeliveryDetails = new SetShippingDetails
            {
                Name = "Set Delivery Details",
                UrlSegment = "set-delivery-details",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 2,
                PublishOn = DateTime.UtcNow,
            };
            _documentService.AddDocument(setDeliveryDetails);
            var orderPlaced = new OrderPlaced
            {
                Name = "Order Placed",
                UrlSegment = "order-placed",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 3,
                PublishOn = DateTime.UtcNow,
            };
            _documentService.AddDocument(orderPlaced);

            //Added to cart
            var addedToCart = new ProductAddedToCart()
            {
                Name = "Added to Basket",
                UrlSegment = "add-to-basket",
                RevealInNavigation = false,
                PublishOn = DateTime.UtcNow
            };
            _documentService.AddDocument(addedToCart);

            //add currency
            var britishCurrency = new Entities.Currencies.Currency
            {
                Name = "British Pound",
                Code = "GBP",
                Format = "£0.00"
            };
            _currencyService.Add(britishCurrency);
        }

        private static string GetFooterLinksText()
        {
            return @"<ul class=""inline""><li><a href=""#"">About &bull;</a></li><li><a href=""#"">Contact Us &bull;</a></li><li><a href=""#"">Privacy Policy &amp; Cookie Info &bull;</a></li><li><a href=""#"">Mr CMS</a></li></ul>";
        }
    }
}