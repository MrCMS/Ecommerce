using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class WarehouseStockController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IWarehouseStockAdminService _warehouseStockAdminService;

        public WarehouseStockController(IWarehouseStockAdminService warehouseStockAdminService)
        {
            _warehouseStockAdminService = warehouseStockAdminService;
        }

        [MrCMSACLRule(typeof(WarehouseStockACL), WarehouseStockACL.List)]
        public ViewResult Index(WarehouseStockSearchModel stockSearchModel)
        {
            ViewData["results"] = _warehouseStockAdminService.Search(stockSearchModel);
            ViewData["warehouse-options"] = _warehouseStockAdminService.GetWarehouseOptions();
            return View(stockSearchModel);
        }
        [HttpGet]
        [MrCMSACLRule(typeof(WarehouseStockACL), WarehouseStockACL.Edit)]
        public ViewResult Edit(WarehouseStock stock)
        {
            return View(stock);
        }
        [HttpPost]
        [ActionName("Edit")]
        [MrCMSACLRule(typeof(WarehouseStockACL), WarehouseStockACL.Edit)]
        public RedirectToRouteResult Edit_POST(WarehouseStock stock)
        {
            _warehouseStockAdminService.Update(stock);
            return RedirectToAction("Index", new {sku = stock.ProductVariant.SKU});
        }
    }
}