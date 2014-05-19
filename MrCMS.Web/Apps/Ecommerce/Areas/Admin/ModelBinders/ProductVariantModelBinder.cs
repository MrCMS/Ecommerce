using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
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

                var methodKeys =
                    controllerContext.HttpContext.Request.Params.AllKeys.Where(s => s.StartsWith(ShippingMethodPrefix))
                        .ToList();

                var excludedMethods = new List<ShippingMethod>();
                foreach (var key in methodKeys)
                {
                    var method = Session.Get<ShippingMethod>(Convert.ToInt32(key.Replace(ShippingMethodPrefix, "")));
                    excludedMethods.Add(method);
                }
                productVariant.RestrictedShippingMethods = excludedMethods;
            }
            return bindModel;
        }
    }
}