using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GetGiftCardTypeOptions : IGetGiftCardTypeOptions
    {
        public List<SelectListItem> Get()
        {
            return Enum.GetValues(typeof (GiftCardType))
                .Cast<GiftCardType>()
                .BuildSelectItemList(type => type.ToString(), emptyItem: null);
        }
    }
}