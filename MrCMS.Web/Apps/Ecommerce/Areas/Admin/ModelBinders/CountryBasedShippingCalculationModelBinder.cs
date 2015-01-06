using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class CountryBasedShippingCalculationModelBinder : MrCMSDefaultModelBinder
    {
        public CountryBasedShippingCalculationModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var calculation = base.BindModel(controllerContext, bindingContext) as CountryBasedShippingCalculation;

            if (calculation == null)
                return null;

            IEnumerable<string> countryKeys =
                controllerContext.HttpContext.Request.Form.AllKeys.Where(key => key.StartsWith("country-"));

            List<string> codes =
                (from key in countryKeys
                    where controllerContext.GetValueFromRequest(key).Contains("true")
                    select key.Split('-')[1]).ToList();

            calculation.Countries = string.Join(",", codes);
            return calculation;
        }
    }
}