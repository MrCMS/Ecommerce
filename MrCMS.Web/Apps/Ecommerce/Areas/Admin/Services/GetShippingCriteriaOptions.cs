using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GetShippingCriteriaOptions : IGetShippingCriteriaOptions
    {
        public List<SelectListItem> Get()
        {
            return Enum.GetValues(typeof (ShippingCriteria)).Cast<ShippingCriteria>()
                .BuildSelectItemList(criteria => EnumHelper.GetDescription(criteria),
                    criteria => criteria.ToString(), emptyItem: null);
        }
    }
}