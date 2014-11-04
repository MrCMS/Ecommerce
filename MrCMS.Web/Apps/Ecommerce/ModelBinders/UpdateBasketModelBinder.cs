using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.ModelBinders
{
    public class UpdateBasketModelBinder : MrCMSDefaultModelBinder
    {
        public UpdateBasketModelBinder(IKernel kernel) : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var cartUpdateValues = new List<CartUpdateValue>();

            var splitQuantities = (controllerContext.HttpContext.Request["quantities"] ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            splitQuantities.ForEach(s =>
                                    {
                                        var strings = s.Split(':');
                                        cartUpdateValues.Add(new CartUpdateValue
                                                             {
                                                                 ItemId = Convert.ToInt32((string) strings[0]),
                                                                 Quantity = Convert.ToInt32((string) strings[1])
                                                             });
                                    });
            return cartUpdateValues;
        }
    }
}