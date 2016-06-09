using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ItemIsInCategoryController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICategoryAdminService _categoryAdminService;

        public ItemIsInCategoryController(ICategoryAdminService categoryAdminService)
        {
            _categoryAdminService = categoryAdminService;
        }

        public PartialViewResult Fields(ItemIsInCategory itemIsInCategory)
        {
            return PartialView(itemIsInCategory);
        }

        [HttpGet]
        public PartialViewResult SearchCategories(int page = 1, string query = "")
        {
            var items = _categoryAdminService.Search(query, page);
            return PartialView(items);
        }
    }
}