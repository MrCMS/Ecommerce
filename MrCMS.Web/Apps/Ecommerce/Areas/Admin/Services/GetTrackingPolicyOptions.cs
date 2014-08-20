using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GetTrackingPolicyOptions : IGetTrackingPolicyOptions
    {
        public List<SelectListItem> Get()
        {
            return Enum.GetValues(typeof(TrackingPolicy)).Cast<TrackingPolicy>()
                .BuildSelectItemList(policy => EnumHelper.GetDescription(policy),
                    policy => policy.ToString(), emptyItem: null);
        }
    }
}