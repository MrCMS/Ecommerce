using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.UrlGenerators;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupEcommerceLayouts : ISetupEcommerceLayouts
    {
        private readonly IDocumentService _documentService;
        private readonly ILayoutAreaAdminService _layoutAreaAdminService;
        private readonly IPageDefaultsAdminService _pageDefaultsAdminService;
        private readonly IPageTemplateAdminService _pageTemplateAdminService;

        public SetupEcommerceLayouts(IDocumentService documentService, ILayoutAreaAdminService layoutAreaAdminService,
            IPageTemplateAdminService pageTemplateAdminService,
            IPageDefaultsAdminService pageDefaultsAdminService)
        {
            _documentService = documentService;
            _layoutAreaAdminService = layoutAreaAdminService;
            _pageTemplateAdminService = pageTemplateAdminService;
            _pageDefaultsAdminService = pageDefaultsAdminService;
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
                new LayoutArea {AreaName = "Footer Area 1", Layout = eCommerceLayout},
                new LayoutArea {AreaName = "Footer Area 2", Layout = eCommerceLayout},
                new LayoutArea {AreaName = "Footer Area 3", Layout = eCommerceLayout},
                new LayoutArea {AreaName = "Footer Area 4", Layout = eCommerceLayout}
            };

            _documentService.AddDocument(eCommerceLayout);
            foreach (LayoutArea area in ecommerceLayoutArea)
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
            foreach (LayoutArea area in homeLayoutAreas)
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
                new LayoutArea {AreaName = "Checkout Header Left", Layout = checkoutLayout},
                new LayoutArea {AreaName = "Checkout Header Middle", Layout = checkoutLayout},
                new LayoutArea {AreaName = "Checkout Header Right", Layout = checkoutLayout},
                new LayoutArea {AreaName = "Checkout Footer Left", Layout = checkoutLayout},
                new LayoutArea {AreaName = "Checkout Footer Right", Layout = checkoutLayout}
            };
            _documentService.AddDocument(checkoutLayout);

            foreach (LayoutArea area in checkoutLayoutAreas)
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
                new LayoutArea {AreaName = "Below Product Information", Layout = productLayout},
                new LayoutArea {AreaName = "Before Product Content", Layout = productLayout},
                new LayoutArea {AreaName = "After Product Content", Layout = productLayout}
            };

            _documentService.AddDocument(productLayout);
            foreach (LayoutArea area in productLayoutAreas)
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
            foreach (LayoutArea area in searchLayoutAreas)
                _layoutAreaAdminService.SaveArea(area);
            layoutModel.SearchLayout = searchLayout;

            var contentLayout = new Layout
            {
                Name = "Content Layout",
                UrlSegment = "_ContentLayout",
                LayoutAreas = new List<LayoutArea>(),
                Parent = eCommerceLayout
            };
            _documentService.AddDocument(contentLayout);
            layoutModel.ContentLayout = contentLayout;

            // UserAccount
            var userAccountLayout = new Layout
            {
                Name = "User Account Layout",
                UrlSegment = "_UserAccountLayout",
                LayoutAreas = new List<LayoutArea>(),
                Parent = eCommerceLayout
            };
            var userAccountAreas = new List<LayoutArea>
            {
                new LayoutArea {AreaName = "Right Column", Layout = userAccountLayout}
            };
            _documentService.AddDocument(userAccountLayout);
            foreach (LayoutArea area in userAccountAreas)
                _layoutAreaAdminService.SaveArea(area);
            layoutModel.UserAccountLayout = userAccountLayout;

            //Page templates
            var homeTemplate = new PageTemplate
            {
                Name = "Home Page",
                PageType = typeof (TextPage).FullName,
                UrlGeneratorType = typeof(DefaultWebpageUrlGenerator).FullName,
                Layout = homeLayout
            };
            _pageTemplateAdminService.Add(homeTemplate);

            SetPageDefaults(layoutModel);

            return layoutModel;
        }

        private void SetPageDefaults(LayoutModel layoutModel)
        {
            //product search
            string generatorTypeName = typeof(DefaultWebpageUrlGenerator).FullName;

            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(ProductSearch).FullName,
                LayoutId = layoutModel.SearchLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            //checkout
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(EnterOrderEmail).FullName,
                LayoutId = layoutModel.CheckoutLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(OrderPlaced).FullName,
                LayoutId = layoutModel.CheckoutLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(PaymentDetails).FullName,
                LayoutId = layoutModel.CheckoutLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(SetShippingDetails).FullName,
                LayoutId = layoutModel.CheckoutLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            // product
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(Product).FullName,
                LayoutId = layoutModel.ProductLayout.Id,
                GeneratorTypeName = typeof(ProductUrlGenerator).FullName
            });
            // category
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(Category).FullName,
                GeneratorTypeName = typeof(CategoryWithHierarchyUrlGenerator).FullName
            });
            // brand
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(Brand).FullName,
                GeneratorTypeName = typeof(BrandUrlGenerator).FullName
            });


            // UserAccount Pages

            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(UserAccountInfo).FullName,
                LayoutId = layoutModel.UserAccountLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(UserAccountChangePassword).FullName,
                LayoutId = layoutModel.UserAccountLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(UserAccountAddresses).FullName,
                LayoutId = layoutModel.UserAccountLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(UserAccountOrders).FullName,
                LayoutId = layoutModel.UserAccountLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(UserAccountReviews).FullName,
                LayoutId = layoutModel.UserAccountLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
            _pageDefaultsAdminService.SetDefaults(new DefaultsInfo
            {
                PageTypeName = typeof(UserAccountRewardPoints).FullName,
                LayoutId = layoutModel.UserAccountLayout.Id,
                GeneratorTypeName = generatorTypeName
            });
        }
    }
}