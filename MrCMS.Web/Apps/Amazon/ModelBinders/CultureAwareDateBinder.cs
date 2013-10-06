using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.ModelBinders
{
    public class CultureAwareDateBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext", "controllerContext is null.");
            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext", "bindingContext is null.");

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null)
                throw new ArgumentNullException(bindingContext.ModelName);



            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            try
            {
                var date = value.ConvertTo(typeof(DateTime), CurrentRequestData.CultureInfo);

                return date;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
                return null;
            }
        }
    }

    public class NullableCultureAwareDateBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext", "controllerContext is null.");
            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext", "bindingContext is null.");

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value == null) return null;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            try
            {
                var date = value.ConvertTo(typeof(DateTime), CurrentRequestData.CultureInfo);

                return date;
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
                return null;
            }
        }
    }
}