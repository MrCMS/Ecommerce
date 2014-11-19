using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class ProductReviewApprovalModelBinder : MrCMSDefaultModelBinder
    {
        public ProductReviewApprovalModelBinder(IKernel kernel) : base(kernel)
        {
            
        }
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if (model is Review)
            {
                if (controllerContext.HttpContext.Request.Form["Approve"] != null)
                {
                    (model as Review).Approved = true;
                }
                if (controllerContext.HttpContext.Request.Form["Reject"] != null)
                {
                    (model as Review).Approved = false;
                }
            }
            return model;
        }
    }
}