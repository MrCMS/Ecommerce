using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductConditionService : IProductConditionService
    {
        public List<SelectListItem> GetOptions()
        {
            return
                Enum.GetValues(typeof(ProductCondition))
                    .Cast<ProductCondition>()
                    .BuildSelectItemList(item => GeneralHelper.GetDescriptionFromEnum(item),item=>item.ToString(), emptyItem: null);
        }
    }
}