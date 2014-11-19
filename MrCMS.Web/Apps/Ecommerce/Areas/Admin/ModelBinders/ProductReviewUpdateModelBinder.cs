using System;
using System.Linq;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class ProductReviewUpdateModelBinder : MrCMSDefaultModelBinder
    {
        public ProductReviewUpdateModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            var currentOperation = "";

            if (controllerContext.HttpContext.Request.Form["Approve"] != null)
            {
                currentOperation = "Approve";
            }
            if (controllerContext.HttpContext.Request.Form["Reject"] != null)
            {
                currentOperation = "Reject";
            }
            if (controllerContext.HttpContext.Request.Form["Delete"] != null)
            {
                currentOperation = "Delete";
            }

            var nameValueCollection = controllerContext.HttpContext.Request.Form;

            var keys = nameValueCollection.AllKeys.Where(s => s.StartsWith("review-"));

            return keys.Select(s =>
            {
                var substring = s.Substring(7);
                return new ReviewUpdateModel
                {
                    ReviewId = Convert.ToInt32(substring),
                    Approved = nameValueCollection[s].Contains("true"),
                    CurrentOperation = currentOperation
                };
            }).ToList();
        }
    }
}