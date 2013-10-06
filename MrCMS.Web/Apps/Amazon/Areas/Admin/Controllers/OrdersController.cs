using System;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Web.Mvc;
using MrCMS.Web.Apps.Amazon.Services.Orders;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class OrdersController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly IAmazonOrderSyncManager _syncAmazonOrderService;
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly IOptionService _optionService;
        private readonly IAmazonOrderSearchService _amazonOrderSearchService;
        private readonly EcommerceSettings _ecommerceSettings;

        public OrdersController(IAmazonOrderSyncManager syncAmazonOrderService,
            IAmazonOrderService amazonOrderService, AmazonAppSettings amazonAppSettings,
            IOptionService optionService, IAmazonOrderSearchService amazonOrderSearchService, EcommerceSettings ecommerceSettings)
        {
            _syncAmazonOrderService = syncAmazonOrderService;
            _amazonOrderService = amazonOrderService;
            _amazonAppSettings = amazonAppSettings;
            _optionService = optionService;
            _amazonOrderSearchService = amazonOrderSearchService;
            _ecommerceSettings = ecommerceSettings;
        }

        [HttpGet]
        public ActionResult Index(AmazonOrderSearchModel model)
        {
            ViewData["AmazonManageOrdersUrl"] = _amazonAppSettings.AmazonManageOrdersUrl;
            return View(PrepareModel(model));
        }

        [HttpGet]
        public ActionResult Orders(AmazonOrderSearchModel model)
        {
            return PartialView(PrepareModel(model));
        }

        private AmazonOrderSearchModel PrepareModel(AmazonOrderSearchModel model)
        {
            ViewData["AmazonOrderDetailsUrl"] = _amazonAppSettings.AmazonOrderDetailsUrl;
            ViewData["ShippingStatuses"] = GeneralHelper.GetEnumOptionsWithEmpty<ShippingStatus>();
            model.Results = new PagedList<AmazonOrder>(null, 1, _ecommerceSettings.PageSizeAdmin);
            try
            {
                model.Results = _amazonOrderSearchService.Search(model, model.Page, _ecommerceSettings.PageSizeAdmin);
            }
            catch (Exception)
            {
                model.Results = _amazonOrderService.Search();
            }
            return model;
        }

        [HttpGet]
        public ActionResult Details(AmazonOrder amazonOrder)
        {
            if (amazonOrder != null)
                return View(amazonOrder);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SyncMany()
        {
            return View(new AmazonSyncModel());
        }

        [HttpGet]
        public ActionResult SyncOne(AmazonOrder amazonOrder)
        {
            if (amazonOrder != null)
                return View(new AmazonSyncModel() { Id = amazonOrder.Id, Description = amazonOrder.AmazonOrderId });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Sync(AmazonSyncModel model)
        {
            if (model != null)
            {
                //_syncAmazonOrderService.SyncAmazonOrders(model);
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        [ActionName("GetNewOrders")]
        public ViewResult GetNewOrders_GET(GetUpdatedOrdersRequest request)
        {
            ViewData["result"] = TempData["result"];
            return View(request);
        }

        [HttpPost]
        public RedirectToRouteResult GetNewOrders(GetUpdatedOrdersRequest request)
        {
            TempData["result"] = _syncAmazonOrderService.GetUpdatedInfoFromAmazon(request);
            return RedirectToAction("GetNewOrders");
        }

        [HttpGet]
        [ActionName("ShipOne")]
        public ActionResult ShipOne_GET(AmazonOrder amazonOrder)
        {
            if (amazonOrder != null)
                return View(new AmazonSyncModel { Id = amazonOrder.Id, Description = amazonOrder.AmazonOrderId });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ShipOne(AmazonSyncModel model)
        {
            if (model != null)
            {
                //_syncAmazonOrderService.ShipOrder(model);
                return Json(true);
            }
            return Json(false);
        }
    }
}
