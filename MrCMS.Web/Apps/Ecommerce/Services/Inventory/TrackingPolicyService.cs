using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Helpers;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public class TrackingPolicyService : ITrackingPolicyService
    {
        public List<SelectListItem> GetOptions()
        {
            return
                Enum.GetValues(typeof(TrackingPolicy))
                    .Cast<TrackingPolicy>()
                    .BuildSelectItemList(item => GeneralHelper.GetDescriptionFromEnum(item),item=>item.ToString(), emptyItem: null);
        }
    }
}