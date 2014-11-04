using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class ProductOptionModelBinder : MrCMSDefaultModelBinder
    {
        public ProductOptionModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            int id;
            return int.TryParse(GetValueFromContext(controllerContext, "productOptionId"), out id)
                ? Session.Get<ProductOption>(id)
                : null;
        }
    }
}