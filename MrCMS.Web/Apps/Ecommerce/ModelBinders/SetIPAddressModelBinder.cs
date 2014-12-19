using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class SetIPAddressModelBinder : MrCMSDefaultModelBinder
    {
        public SetIPAddressModelBinder(IKernel kernel) : base(kernel)
        {
            
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var bindModel = base.BindModel(controllerContext, bindingContext);

            if (bindModel is IHaveIPAddress)
                (bindModel as IHaveIPAddress).IPAddress = controllerContext.HttpContext.GetCurrentIP();

            return bindModel;
        }
    }

    public interface IHaveIPAddress
    {
        string IPAddress { get; set; }
    }
}