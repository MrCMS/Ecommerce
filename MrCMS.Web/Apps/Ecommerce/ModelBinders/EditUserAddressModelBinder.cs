using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class EditUserAddressModelBinder : MrCMSDefaultModelBinder
    {
        public EditUserAddressModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            int addressId;
            int? id = int.TryParse(GetValueFromContext(controllerContext, "id"), out addressId)
                ? addressId
                : (int?) null;

            if (id != null)
            {
                Address address = Session.Get<Address>(id);
                return address;
            }

            return null;
        }
    }
}