using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface ISetShippingAddressService
    {
        List<SelectListItem> GetCountryOptions();
        List<SelectListItem> GetOtherAddresses();
        Address GetShippingAddress();
        void SetShippingAddress(Address address);
        ActionResult RedirectToDetails();
    }
}