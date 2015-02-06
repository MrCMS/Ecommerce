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
            if (model is ProductReview)
            {
                if (controllerContext.HttpContext.Request.Form["Approve"] != null)
                {
                    (model as ProductReview).Approved = true;
                }
                if (controllerContext.HttpContext.Request.Form["Reject"] != null)
                {
                    (model as ProductReview).Approved = false;
                }
            }
            return model;
        }
    }
}