using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using Ninject;
using MrCMS.Helpers;

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

        public override System.Collections.Generic.IEnumerable<System.Type> BaseTypes
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
            context.MapRoute("Cart - Basic Details", "Apps/Ecommerce/Cart/BasicDetails", new { controller = "Cart", action = "BasicDetails" });
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
            var currentSite = new CurrentSite(site);
            var configurationProvider = new ConfigurationProvider(new SettingService(session), currentSite);
            var siteSettings = configurationProvider.GetSiteSettings<SiteSettings>();
            var documentService = new DocumentService(session, siteSettings, currentSite);

            var productSearch = new ProductSearch
                                    {
                                        Name = "Product Search",
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
                                 UrlSegment = "~/Apps/Ecommerce/Views/Shared/_EcommerceLayout.cshtml",
                                 LayoutAreas = new List<LayoutArea>()
                             };
            var areas = new List<LayoutArea>
                                     {
                                         new LayoutArea {AreaName = "Header", Layout = layout},
                                         new LayoutArea {AreaName = "After Content", Layout = layout},
                                         new LayoutArea {AreaName = "Footer", Layout = layout}
                                     };
            documentService.AddDocument(layout);
            var layoutAreaService = new LayoutAreaService(session);
            foreach (var area in areas)
                layoutAreaService.SaveArea(area);
            siteSettings.DefaultLayoutId = layout.Id;
            configurationProvider.SaveSettings(siteSettings);
        }
    }
}