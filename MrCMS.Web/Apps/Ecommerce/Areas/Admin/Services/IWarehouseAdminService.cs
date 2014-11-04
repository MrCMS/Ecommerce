using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IWarehouseAdminService
    {
        IPagedList<Warehouse> Search(WarehouseSearchModel searchModel);

        void Add(Warehouse warehouse);
        void Update(Warehouse warehouse);
        void Delete(Warehouse warehouse);
        StockGenerationModel GetStockGenerationModel(Warehouse warehouse);
        List<SelectListItem> GetWarehouseOptions(Warehouse warehouse);
        List<SelectListItem> GetStockGenerationTypeOptions(Warehouse warehouse);
        bool AnyStock(Warehouse warehouse);
        GenerateStockResult GenerateStock(StockGenerationModel model);
        IList<Warehouse> ListAll();
    }
}