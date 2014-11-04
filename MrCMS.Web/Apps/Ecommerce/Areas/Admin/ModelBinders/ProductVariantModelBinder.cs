using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class ProductVariantModelBinder : MrCMSDefaultModelBinder
    {
        private readonly ISetVariantTypeProperties _setVariantTypeProperties;
        private readonly ISetRestrictedShippingMethods _setRestrictedShippingMethods;


        public ProductVariantModelBinder(ISetVariantTypeProperties setVariantTypeProperties, ISetRestrictedShippingMethods setRestrictedShippingMethods, IKernel kernel)
            : base(kernel)
        {
            _setVariantTypeProperties = setVariantTypeProperties;
            _setRestrictedShippingMethods = setRestrictedShippingMethods;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var bindModel = base.BindModel(controllerContext, bindingContext);
            if (bindModel is ProductVariant)
            {
                var productVariant = bindModel as ProductVariant;

                var variantType = controllerContext.GetValueFromRequest("VariantType");
                _setVariantTypeProperties.SetProperties(productVariant, variantType);
                _setRestrictedShippingMethods.SetMethods(productVariant, controllerContext.HttpContext.Request.Params);
            }
            return bindModel;
        }
    }
}