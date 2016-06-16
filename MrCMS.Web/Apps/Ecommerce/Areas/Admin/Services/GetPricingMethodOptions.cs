using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GetPricingMethodOptions : IGetPricingMethodOptions
    {
        public List<SelectListItem> GetOptions()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom<IProductPricingMethod>()
                .BuildSelectItemList(type => type.Name.BreakUpString(), type => type.FullName, emptyItem: null);
        }
    }
}