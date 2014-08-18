using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GetProductVariantTypeOptions : IGetProductVariantTypeOptions
    {
        public List<SelectListItem> Get()
        {
            return Enum.GetValues(typeof (VariantType))
                .Cast<VariantType>()
                .BuildSelectItemList(type => type.ToString().BreakUpString(),
                    type => type.ToString(), emptyItem: null);
        }
    }
}