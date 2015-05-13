using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models.UserAccount;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class UserAccountOrdersModelBinder : MrCMSDefaultModelBinder
    {
        public UserAccountOrdersModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            int pageVal;
            int? page = int.TryParse(GetValueFromContext(controllerContext, "page"), out pageVal)
                ? pageVal
                : (int?) null;

            var model = new UserAccountOrdersSearchModel();
            if (page != null)
                model.Page = page.Value;

            return model;
        }
    }
}