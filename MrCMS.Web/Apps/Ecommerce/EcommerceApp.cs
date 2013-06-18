using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using Ninject;

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
                                 new[] { typeof(ProductController).Namespace});
            context.MapRoute("Product Variant - GetPriceBreaksForProductVariant", "Apps/Ecommerce/ProductVariant/GetPriceBreaksForProductVariant", new { controller = "ProductVariant", action = "GetPriceBreaksForProductVariant" });
            context.MapRoute("Product Search - Results", "Apps/Ecommerce/ProductSearch/Results", new { controller = "ProductSearch", action = "Results" });
            context.MapRoute("Category - Results", "Apps/Ecommerce/Category/Results", new { controller = "Category", action = "Results" });
            context.MapRoute("Cart - Details", "Apps/Ecommerce/Cart/Details", new { controller = "Cart", action = "Details" });
        }
    }
}