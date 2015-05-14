using System.Drawing;
using System.Linq;
using System.Text;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Caching;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupEcommerceWidgets : ISetupEcommerceWidgets
    {
        private readonly IWidgetService _widgetService;
        private readonly IDocumentService _documentService;
        private readonly IFormAdminService _formAdminService;
        private readonly IFileService _fileService;

        public SetupEcommerceWidgets(IWidgetService widgetService, IDocumentService documentService, IFormAdminService formAdminService, IFileService fileService)
        {
            _widgetService = widgetService;
            _documentService = documentService;
            _formAdminService = formAdminService;
            _fileService = fileService;
        }

        public void Setup(PageModel pageModel, MediaModel mediaModel, LayoutModel layoutModel)
        {

            SetupEcommerceLayoutWidgets(mediaModel, layoutModel);
            SetupHomeLayoutWidgets(pageModel, mediaModel, layoutModel);
            SetupSearchLayoutWidgets(pageModel, layoutModel);
            SetupCheckoutLayoutWidgets(mediaModel, layoutModel);
            SetupProductLayoutWidgets(layoutModel, pageModel);
        }

        private void SetupProductLayoutWidgets(LayoutModel layoutModel, PageModel pageModel)
        {
            var breadcrumbs = new BreadCrumb
            {
                LayoutArea = layoutModel.ProductLayout.LayoutAreas.Single(x => x.AreaName == "Before Product Content"),
                Name = "Breadcrumbs",
                IsRecursive = true,
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 180
            };
            _widgetService.AddWidget(breadcrumbs);

            var relatedProducts = new RelatedProducts
            {
                LayoutArea = layoutModel.ProductLayout.LayoutAreas.Single(x => x.AreaName == "After Product Content"),
                Name = "Related Products",
                Cache = true,
                CacheExpiryType = CacheExpiryType.Absolute,
                CacheLength = 1800
            };
            _widgetService.AddWidget(relatedProducts);

            var peopleAlsoBought = new PeopleWhoBoughtThisAlsoBought
            {
                LayoutArea = layoutModel.ProductLayout.LayoutAreas.Single(x => x.AreaName == "After Product Content"),
                Name = "People who bought this also bought",
                Cache = true,
                CacheExpiryType = CacheExpiryType.Absolute,
                CacheLength = 1800
            };
            _widgetService.AddWidget(peopleAlsoBought);

            var otherCategories = new NotWhatYouWereLookingForWidget
            {
                LayoutArea = layoutModel.ProductLayout.LayoutAreas.Single(x => x.AreaName == "Below Product Price"),
                Name = "Not what you were looking for?",
                Cache = true,
                CacheExpiryType = CacheExpiryType.Absolute,
                CacheLength = 1800
            };
            _widgetService.AddWidget(otherCategories);
        }

        private void SetupCheckoutLayoutWidgets(MediaModel mediaModel, LayoutModel layoutModel)
        {
            //checkout images
            var checkoutLogo = new LinkedImage
            {
                Image = _fileService.GetFileLocation(mediaModel.Logo, new Size { Width = 200, Height = 200 }),
                Link = "/",
                LayoutArea = layoutModel.CheckoutLayout.LayoutAreas.Single(x => x.AreaName == "Checkout Header Left"),
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };
            _widgetService.AddWidget(checkoutLogo);

            var checkoutSecureBadge = new LinkedImage
            {
                Image = mediaModel.SecureCheckout.FileUrl,
                LayoutArea = layoutModel.CheckoutLayout.LayoutAreas.Single(x => x.AreaName == "Checkout Header Middle"),
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };
            _widgetService.AddWidget(checkoutSecureBadge);
        }

        private void SetupSearchLayoutWidgets(PageModel pageModel, LayoutModel layoutModel)
        {
            var recentlyViewed = new RecentlyViewedItems
            {
                Webpage = pageModel.ProductSearch,
                LayoutArea = layoutModel.SearchLayout.LayoutAreas.FirstOrDefault(x => x.AreaName == "After Filters"),
                Name = "Recently Viewed"
            };
            _widgetService.AddWidget(recentlyViewed);

            //breadcrumb
            var breadcrumbs = new BreadCrumb
            {
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "Before Content"),
                Name = "Breadcrumbs",
                Webpage = pageModel.ProductSearch,
                IsRecursive = true,
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 1800
            };
            _widgetService.AddWidget(breadcrumbs);
        }

        private void SetupHomeLayoutWidgets(PageModel pageModel, MediaModel mediaModel, LayoutModel layoutModel)
        {
            var layout = layoutModel.HomeLayout;
            var beforeContent = layoutModel.EcommerceLayout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Before Content");
            var teaser1Area = layout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Teaser1");
            var teaser2Area = layout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Teaser2");
            var teaser3Area = layout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Teaser3");
            var teaser4Area = layout.LayoutAreas.FirstOrDefault(x => x.AreaName == "Teaser4");

            var slider = new Slider
            {
                Image = mediaModel.SliderImage1.FileUrl,
                Image1 = mediaModel.SliderImage2.FileUrl,
                LayoutArea = beforeContent,
                Webpage = pageModel.HomePage,
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 1800
            };
            _widgetService.AddWidget(slider);

            var featuredProducts = new FeaturedProducts
            {
                LayoutArea = beforeContent,
                Webpage = pageModel.HomePage,
                ListOfFeaturedProducts = GetFeaturedProducts(),
                Name = "Featured Products",
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 1800
            };
            _widgetService.AddWidget(featuredProducts);

            //nav
            var nav = new MobileFriendlyNavigation.Widgets.MobileFriendlyNavigation
            {
                Name = "Navigation",
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == ("Navigation")),
                IncludeChildren = true,
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };
            _widgetService.AddWidget(nav);

            var featuredCategories = new FeaturedCategories
            {
                LayoutArea = beforeContent,
                Webpage = pageModel.HomePage,
                ListOfFeaturedCategories = GetFeaturedCategories(),
                Name = "Featured Categories",
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 1800
            };
            _widgetService.AddWidget(featuredCategories);

            var teaser1 = new TextWidget()
            {
                LayoutArea = teaser1Area,
                Webpage = pageModel.HomePage,
                Text = string.Format(@"<div class=""padding-bottom-10""><span><img src=""{0}"" /> </span></div><h3><a href=""#"">FREE delivery on orders over &pound;50. </a></h3><p>Orders placed Monday to Friday before 2pm will generally be picked and packed for immediate despatch. Please note that orders placed over the weekend or on public holidays will be processed on the next working day.</p>", mediaModel.DeliveryIcon.FileUrl),
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 1800
                
            };
            _widgetService.AddWidget(teaser1);

            var teaser2 = new TextWidget()
            {
                LayoutArea = teaser2Area,
                Webpage = pageModel.HomePage,
                Text = string.Format(@"<div class=""padding-bottom-10""><span><img src=""{0}"" /> </span></div><h3><a href=""#"">7 day no question returns.</a></h3><p>We offer a 28 Day Money Back Guarantee. If for any reason you are not completely delighted with your purchase you may download a Returns Form and return it within 28 days of receipt for a full refund or exchange.</p>", mediaModel.ReturnIcon.FileUrl),
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 1800
                
            };
            _widgetService.AddWidget(teaser2);

            var teaser3 = new TextWidget()
            {
                LayoutArea = teaser3Area,
                Webpage = pageModel.HomePage,
                Text = string.Format(@"<div class=""padding-bottom-10""><span><img src=""{0}"" /> </span></div><h3><a href=""#"">Store locations.</a></h3><p>Use our store locator to find a store near you as well as information like opening times, addresses, maps and a list of facilities available at every store.</p>", mediaModel.LocationIcon.FileUrl),
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 1800
                
            };
            _widgetService.AddWidget(teaser3);

            var teaser4 = new TextWidget()
            {
                LayoutArea = teaser4Area,
                Webpage = pageModel.HomePage,
                Text = string.Format(@"<div class=""padding-bottom-10""><span><img src=""{0}"" /> </span></div><h3><a href=""#"">Contact us.</a></h3><p>Our customer service team is always willing to answer your proposal concerning Samsung Service. Your message will be promptly handled under the direct supervision of our executive management.</p>", mediaModel.ContactIcon.FileUrl),
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };
            _widgetService.AddWidget(teaser4);

            GetFormProperties(pageModel.HomePage);
            //home footer form
            var footerLinksWidgetForm = new TextWidget
            {
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "Footer Area 4"),
                Name = "Footer links",
                Text = string.Format("[form-{0}]", pageModel.HomePage.Id),
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };
            _widgetService.AddWidget(footerLinksWidgetForm);
        }

        private void SetupEcommerceLayoutWidgets(MediaModel mediaModel, LayoutModel layoutModel)
        {
            var linkedImageLogo = new LinkedImage
            {
                Name = "Logo",
                Link = "/",
                Image = mediaModel.Logo.FileUrl,
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "Header Left"),
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };
            _widgetService.AddWidget(linkedImageLogo);
            // Search
            var searchBox = new SearchBox
            {
                Name = "Search",
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "Header Middle"),
                Cache = true,
            };
            _widgetService.AddWidget(searchBox);

            // Ecommerce user links
            var userLinks = new EcommerceUserLinks
            {
                Name = "Ecommerce User Links",
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "User Links")
            };
            _widgetService.AddWidget(userLinks);

            //cart widget
            var cartWidget = new CartWidget
            {
                Name = "Cart Widget",
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "Header Right")
            };
            _widgetService.AddWidget(cartWidget);

            //footer links
            var footerLinksWidget = new TextWidget
            {
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "Footer Area 1"),
                Name = "Footer links",
                Text = EcommerceInstallInfo.FooterText1,
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };
            _widgetService.AddWidget(footerLinksWidget);

            footerLinksWidget = new TextWidget
            {
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "Footer Area 2"),
                Name = "Footer links",
                Text = EcommerceInstallInfo.FooterText2,
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };
            _widgetService.AddWidget(footerLinksWidget);

            footerLinksWidget = new TextWidget
            {
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "Footer Area 3"),
                Name = "Footer links",
                Text = EcommerceInstallInfo.FooterText3,
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };
            _widgetService.AddWidget(footerLinksWidget);

            var afterContentCardsTeaser = new TextWidget
            {
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "After Content"),
                Name = "Footer links",
                Text = EcommerceInstallInfo.AfterContentCardsTeaser,
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60
            };

            _widgetService.AddWidget(afterContentCardsTeaser);

            var page404 = _documentService.GetDocumentByUrl<TextPage>("404");
            var notFoundProducts = new On404SearchWidget
            {
                LayoutArea = layoutModel.EcommerceLayout.LayoutAreas.Single(x => x.AreaName == "After Content"),
                Name = "What about these?",
                Cache = true,
                CacheExpiryType = CacheExpiryType.Sliding,
                CacheLength = 60,
                Webpage = page404
            };
            _widgetService.AddWidget(notFoundProducts);
        }

        private void GetFormProperties(TextPage home)
        {
            home = _documentService.GetDocument<TextPage>(home.Id);
            var name = new TextBox
            {
                Name = "Email",
                LabelText = "Newsletter Signup",
                Required = true,
                DisplayOrder = 1,
                Webpage = home
            };
            _formAdminService.AddFormProperty(name);
            _documentService.SaveDocument(home);
        }

        private string GetFeaturedProducts()
        {
            var product1 = _documentService.GetDocumentByUrl<Product>(FeaturedProductsInfo.Product1Url);
            var product2 = _documentService.GetDocumentByUrl<Product>(FeaturedProductsInfo.Product2Url);
            var product3 = _documentService.GetDocumentByUrl<Product>(FeaturedProductsInfo.Product3Url);
            var product4 = _documentService.GetDocumentByUrl<Product>(FeaturedProductsInfo.Product4Url);
            var ids = new StringBuilder();
            if (product1 != null)
                ids.Append(product1.Id + ",");
            if (product2 != null)
                ids.Append(product2.Id + ",");
            if (product3 != null)
                ids.Append(product3.Id + ",");
            if (product4 != null)
                ids.Append(product4.Id + ",");

            return ids.ToString();
        }

        private string GetFeaturedCategories()
        {
            var cat1 = _documentService.GetDocumentByUrl<Category>(FeaturedCategoriesInfo.Category1Url);
            var cat2 = _documentService.GetDocumentByUrl<Category>(FeaturedCategoriesInfo.Category2Url);
            var cat3 = _documentService.GetDocumentByUrl<Category>(FeaturedCategoriesInfo.Category3Url);
            var cat4 = _documentService.GetDocumentByUrl<Category>(FeaturedCategoriesInfo.Category4Url);
            var ids = new StringBuilder();
            if (cat1 != null)
                ids.Append(cat1.Id + ",");
            if (cat2 != null)
                ids.Append(cat2.Id + ",");
            if (cat3 != null)
                ids.Append(cat3.Id + ",");
            if (cat4 != null)
                ids.Append(cat4.Id + ",");

            return ids.ToString();
        }
    }
}