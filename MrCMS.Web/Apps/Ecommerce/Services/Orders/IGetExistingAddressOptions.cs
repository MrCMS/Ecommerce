using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public interface IGetExistingAddressOptions
    {
        List<SelectListItem> Get(IAddress addressToExclude);
    }
}