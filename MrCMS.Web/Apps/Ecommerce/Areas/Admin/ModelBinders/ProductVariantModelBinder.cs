using System;
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
        private readonly ISetETagService _setETagService;


        public ProductVariantModelBinder(ISetVariantTypeProperties setVariantTypeProperties, ISetRestrictedShippingMethods setRestrictedShippingMethods, ISetETagService setETagService, IKernel kernel)
            : base(kernel)
        {
            _setVariantTypeProperties = setVariantTypeProperties;
            _setRestrictedShippingMethods = setRestrictedShippingMethods;
            _setETagService = setETagService;
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

                var eTag = 0;
                Int32.TryParse(controllerContext.GetValueFromRequest("ETag"), out eTag);
                if(eTag > 0)
                    _setETagService.SetETag(productVariant, eTag);
            }
            return bindModel;
        }
    }
}