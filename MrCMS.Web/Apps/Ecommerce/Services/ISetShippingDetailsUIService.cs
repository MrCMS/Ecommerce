using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface ISetShippingDetailsUIService
    {
        CartModel Cart { get; }
        bool UserRequiresRedirect();
        ActionResult UserRedirect();

    }
}