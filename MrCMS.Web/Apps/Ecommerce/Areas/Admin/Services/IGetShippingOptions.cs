using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IGetShippingOptions
    {
        List<SelectListItem> Get(ProductVariant productVariant);
    }
}