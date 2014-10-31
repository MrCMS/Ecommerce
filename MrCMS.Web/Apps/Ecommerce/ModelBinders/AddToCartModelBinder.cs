using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class AddToCartModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IGetGiftCardDataFromContext _getGiftCardDataFromContext;

        public AddToCartModelBinder(IKernel kernel, IGetGiftCardDataFromContext getGiftCardDataFromContext)
            : base(kernel)
        {
            _getGiftCardDataFromContext = getGiftCardDataFromContext;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var bindModel = base.BindModel(controllerContext, bindingContext);
            var addToCartModel = bindModel as AddToCartModel;
            if (addToCartModel != null && addToCartModel.ProductVariant != null)
            {
                if (addToCartModel.ProductVariant.IsGiftCard)
                {
                    addToCartModel.Data = _getGiftCardDataFromContext.Get(controllerContext);
                }
            }
            return bindModel;
        }
    }
}