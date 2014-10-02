using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IWarehouseStockAdminService
    {
        IPagedList<WarehouseStock> Search(WarehouseStockSearchModel stockSearchModel);
        List<SelectListItem> GetWarehouseOptions();
        void Update(WarehouseStock stock);
    }
}