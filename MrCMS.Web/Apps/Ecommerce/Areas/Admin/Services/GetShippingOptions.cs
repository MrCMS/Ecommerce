using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class GetShippingOptions : IGetShippingOptions
    {
        private readonly IShippingMethodAdminService _shippingMethodAdminService;

        public GetShippingOptions(IShippingMethodAdminService shippingMethodAdminService)
        {
            _shippingMethodAdminService = shippingMethodAdminService;
        }

        public List<SelectListItem> Get(ProductVariant productVariant)
        {
            return _shippingMethodAdminService.GetAll()
                .BuildSelectItemList(info => info.Name,
                    info => info.Type, info => productVariant.RestrictedTo.Contains(info.Type), emptyItem: null);
        }
    }
}