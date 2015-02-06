using System;
using System.Collections.Generic;
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
        public ProductReviewUpdateModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        public override object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            ProductReviewOperation currentOperation;

            if (controllerContext.HttpContext.Request.Form["Approve"] != null)
            {
                currentOperation = ProductReviewOperation.Approve;
            }
            else if (controllerContext.HttpContext.Request.Form["Reject"] != null)
            {
                currentOperation = ProductReviewOperation.Reject;
            }
            else if (controllerContext.HttpContext.Request.Form["Delete"] != null)
            {
                currentOperation = ProductReviewOperation.Delete;
            }
            else
            {
                return new ReviewUpdateModel();
            }

            var nameValueCollection = controllerContext.HttpContext.Request.Form;

            var keys = nameValueCollection.AllKeys.Where(s => s.StartsWith("review-"));
            var reviewUpdateModel = new ReviewUpdateModel
            {
                CurrentOperation = currentOperation,
                Reviews = new List<ProductReview>()
            };
            foreach (var key in keys)
            {
                var substring = key.Substring(7);
                int id;
                if (Int32.TryParse(substring, out id) && nameValueCollection[key].Contains("true"))
                {
                    var review = Session.Get<ProductReview>(id);
                    if (review != null)
                        reviewUpdateModel.Reviews.Add(review);
                }

            }
            return reviewUpdateModel;
        }
    }
}