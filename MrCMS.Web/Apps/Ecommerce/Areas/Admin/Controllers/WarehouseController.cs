using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class WarehouseController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IWarehouseAdminService _warehouseAdminService;

        public WarehouseController(IWarehouseAdminService warehouseAdminService)
        {
            _warehouseAdminService = warehouseAdminService;
        }

        [MrCMSACLRule(typeof(WarehouseACL), WarehouseACL.List)]
        public ViewResult Index(WarehouseSearchModel searchModel)
        {
            ViewData["results"] = _warehouseAdminService.Search(searchModel);
            return View(searchModel);
        }

        [HttpGet]
        [MrCMSACLRule(typeof(WarehouseACL), WarehouseACL.Add)]
        public PartialViewResult Add()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("Add")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(WarehouseACL), WarehouseACL.Add)]
        public RedirectToRouteResult Add_POST(Warehouse warehouse)
        {
            _warehouseAdminService.Add(warehouse);
            return RedirectToAction("Edit", new { id = warehouse.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(WarehouseACL), WarehouseACL.Edit)]
        public ViewResult Edit(Warehouse warehouse)
        {
            ViewData["any-stock"] = _warehouseAdminService.AnyStock(warehouse);
            return View(warehouse);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(WarehouseACL), WarehouseACL.Edit)]
        public RedirectToRouteResult Edit_POST(Warehouse warehouse)
        {
            _warehouseAdminService.Update(warehouse);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [MrCMSACLRule(typeof(WarehouseACL), WarehouseACL.Delete)]
        public PartialViewResult Delete(Warehouse warehouse)
        {
            return PartialView(warehouse);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(WarehouseACL), WarehouseACL.Delete)]
        public RedirectToRouteResult Delete_POST(Warehouse warehouse)
        {
            _warehouseAdminService.Delete(warehouse);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GenerateStock(Warehouse warehouse)
        {
            var stockGenerationModel = _warehouseAdminService.GetStockGenerationModel(warehouse);
            ViewData["warehouse-options"] = _warehouseAdminService.GetWarehouseOptions(warehouse);
            ViewData["stock-generation-type-options"] = _warehouseAdminService.GetStockGenerationTypeOptions(warehouse);
            return PartialView(stockGenerationModel);
        }

        [HttpPost]
        public ActionResult GenerateStock(StockGenerationModel model)
        {
            var generateStock = _warehouseAdminService.GenerateStock(model);
            if (generateStock.IsSuccess)
                TempData.SuccessMessages().Add(generateStock.Message);
            else
                TempData.ErrorMessages().Add(generateStock.Message);
            return RedirectToAction("Edit", new { id = model.WarehouseId });
        }

        public ActionResult Reference()
        {
            return PartialView(_warehouseAdminService.ListAll());
        }
    }
}