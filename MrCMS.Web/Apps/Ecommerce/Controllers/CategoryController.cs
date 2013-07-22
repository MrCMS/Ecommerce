using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using System.Collections.Generic;
using System;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Settings;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CategoryController : MrCMSAppUIController<EcommerceApp>
    {
        public ViewResult Show(Category page, [IoCModelBinder(typeof(ProductSearchQueryModelBinder))]ProductSearchQuery query)
        {
            query.CategoryId = page.Id;
            ViewData["query"] = query;
            return View(page);
        }
    }
}