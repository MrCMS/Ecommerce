using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupEcommerceLayouts : ISetupEcommerceLayouts
    {
        private readonly IDocumentService _documentService;
        private readonly ILayoutAreaAdminService _layoutAreaAdminService;
        private readonly IWidgetService _widgetService;
        private readonly IPageTemplateAdminService _pageTemplateAdminService;

        public SetupEcommerceLayouts(IDocumentService documentService, ILayoutAreaAdminService layoutAreaAdminService, IWidgetService widgetService, IPageTemplateAdminService pageTemplateAdminService)
        {
            _documentService = documentService;
            _layoutAreaAdminService = layoutAreaAdminService;
            _widgetService = widgetService;
            _pageTemplateAdminService = pageTemplateAdminService;
        }

        public LayoutModel Setup(MediaModel mediaModel)
        {
            var layoutModel = new LayoutModel();
            //base layout
            var baseLayout = new Layout
            {
                Name = "Base Ecommerce Layout",
                UrlSegment = "_BaseLayout",
                LayoutAreas = new List<LayoutArea>(),
            };
            _documentService.AddDocument(baseLayout);

            //ecommerce main layout
            var eCommerceLayout = new Layout
            {
                Name = "Ecommerce Layout",
                UrlSegment = "_EcommerceLayout",
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

            layoutModel.EcommerceLayout = eCommerceLayout;

            var homeLayout = new Layout
            {
                Name = "Home Layout",
                UrlSegment = "_HomeLayout",
                LayoutAreas = new List<LayoutArea>(),
                Parent = eCommerceLayout
            };
            var homeLayoutAreas = new List<LayoutArea>
            {
                new LayoutArea {AreaName = "Home After Content", Layout = homeLayout},
                new LayoutArea {AreaName = "Teaser1", Layout = homeLayout},
                new LayoutArea {AreaName = "Teaser2", Layout = homeLayout},
                new LayoutArea {AreaName = "Teaser3", Layout = homeLayout},
                new LayoutArea {AreaName = "Teaser4", Layout = homeLayout}
            };
            _documentService.AddDocument(homeLayout);
            foreach (var area in homeLayoutAreas)
                _layoutAreaAdminService.SaveArea(area);
            layoutModel.HomeLayout = homeLayout;
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
            layoutModel.CheckoutLayout = checkoutLayout;
            //product layout
            var productLayout = new Layout
            {
                Name = "Product Layout",
                UrlSegment = "_ProductLayout",
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
            layoutModel.ProductLayout = productLayout;
            //category/search layout
            var searchLayout = new Layout
            {
                Name = "Search Layout",
                UrlSegment = "_SearchLayout",
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
            layoutModel.SearchLayout = searchLayout;
            var linkedImageLogo = new LinkedImage
            {
                Name = "Logo",
                Link = "/",
                Image = mediaModel.Logo.FileUrl,
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
            var nav = new MobileFriendlyNavigation.Widgets.MobileFriendlyNavigation
            {
                Name = "Navigation",
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == ("Navigation")),
                IncludeChildren = true
            };
            _widgetService.AddWidget(nav);

            //footer links
            var footerLinksWidget = new TextWidget
            {
                LayoutArea = ecommerceLayoutArea.Single(x => x.AreaName == "Footer"),
                Name = "Footer links",
                Text = EcommerceInstalInfo.FooterText
            };
            _widgetService.AddWidget(footerLinksWidget);


            //Page templates
            var homeTemplate = new PageTemplate
            {
                Name = "Home Page",
                PageType = "MrCMS.Web.Apps.Core.Pages.TextPage",
                UrlGeneratorType = "MrCMS.Services.DefaultWebpageUrlGenerator",
                Layout = homeLayout
            };
            _pageTemplateAdminService.Add(homeTemplate);

            return layoutModel;
        }
    }
}