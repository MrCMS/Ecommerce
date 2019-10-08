using System.Collections.Generic;
using System.IO;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IBulkOrdersAdminService
    {
        MarkOrdersAsShippedViewModel GetModel(SelectedOrdersViewModel model);
        List<PickingListViewModel> GetPickingList(SelectedOrdersViewModel model);
    }
}