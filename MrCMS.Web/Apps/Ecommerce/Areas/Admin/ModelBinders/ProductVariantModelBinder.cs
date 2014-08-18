using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class ProductVariantModelBinder : MrCMSDefaultModelBinder
    {
        private const string ShippingMethodPrefix = "shipping-method-";


        public ProductVariantModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var bindModel = base.BindModel(controllerContext, bindingContext);
            if (bindModel is ProductVariant)
            {
                var productVariant = bindModel as ProductVariant;

                var fromRequest = controllerContext.GetValueFromRequest("VariantType");
                var value = Enum.Parse(typeof (VariantType), fromRequest);
                var valueFromRequest = value is VariantType ? (VariantType) value : VariantType.Standard;
                switch (valueFromRequest)
                {
                    case VariantType.Standard:
                        productVariant.IsDownloadable = false;
                        productVariant.IsGiftCard = false;
                        break;
                    case VariantType.GiftCard:
                        productVariant.IsDownloadable = false;
                        productVariant.IsGiftCard = true;
                        break;
                    case VariantType.Download:
                        productVariant.IsDownloadable = true;
                        productVariant.IsGiftCard = false;
                        break;
                }
            }
            return bindModel;
        }
    }
}