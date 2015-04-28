using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class UserAccountRewardPointsModelBinder : MrCMSDefaultModelBinder
    {
        public UserAccountRewardPointsModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            int pageVal;
            int? page = int.TryParse(GetValueFromContext(controllerContext, "page"), out pageVal)
                ? pageVal
                : (int?) null;

            var model = new UserAccountRewardPointsSearchModel();
            if (page != null)
                model.Page = page.Value;

            return model;
        }
    }
}