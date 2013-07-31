using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.Misc
{
    public class OptionService : IOptionService
    {
        public List<SelectListItem> GetEnumOptions<T>() where T:struct
        {
            return
                Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .BuildSelectItemList(item => GeneralHelper.GetDescriptionFromEnum(item as Enum),item=>item.ToString(), emptyItem: null);
        }
    }
}