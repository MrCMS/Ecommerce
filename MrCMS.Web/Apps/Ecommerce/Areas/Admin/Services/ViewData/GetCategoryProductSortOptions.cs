using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.ViewData
{
    public class GetCategoryProductSortOptions : BaseAssignWebpageAdminViewData<Category>
    {
        public override void AssignViewData(Category webpage, ViewDataDictionary viewData)
        {
            viewData["product-search-sort-options"] =
                Enum.GetValues(typeof(ProductSearchSort))
                    .Cast<ProductSearchSort>()
                    .BuildSelectItemList(sort => EnumHelper.GetDescription(sort), sort => sort.ToString(),
                        sort => sort == webpage.DefaultProductSearchSort, emptyItemText: "System default");
        }
    }
}