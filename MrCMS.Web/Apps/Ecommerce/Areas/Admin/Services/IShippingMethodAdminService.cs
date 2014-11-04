using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IShippingMethodAdminService
    {
        List<ShippingMethodInfo> GetAll();
        void UpdateSettings(ShippingMethodSettings settings);
    }
}