using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IAvailableBrandsService
    {
        List<SelectListItem> GetAvailableBrands(ProductSearchQuery query);
    }
}