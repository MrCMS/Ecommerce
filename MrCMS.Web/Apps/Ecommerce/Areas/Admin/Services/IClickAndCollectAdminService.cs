using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IClickAndCollectAdminService
    {
        ClickAndCollectSettings GetSettings();
        void Save(ClickAndCollectSettings settings);
    }
}