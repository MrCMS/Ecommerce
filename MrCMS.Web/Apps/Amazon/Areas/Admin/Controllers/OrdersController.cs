using System;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Website.Controllers;
using System.Web.Mvc;
using MrCMS.Web.Apps.Amazon.Services.Orders;

namespace MrCMS.Web.Apps.Amazon.Areas.Admin.Controllers
{
    public class OrdersController : MrCMSAppAdminController<AmazonApp>
    {
        private readonly ISyncAmazonOrderService _syncAmazonOrderService;
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly IOptionService _optionService;
        private readonly IAmazonOrderSearchService _amazonOrderSearchService;

        public OrdersController(ISyncAmazonOrderService syncAmazonOrderService, 
            IAmazonOrderService amazonOrderService, AmazonAppSettings amazonAppSettings, 
            IOptionService optionService, IAmazonOrderSearchService amazonOrderSearchService)
        {
            _syncAmazonOrderService = syncAmazonOrderService;
            _amazonOrderService = amazonOrderService;
            _amazonAppSettings = amazonAppSettings;
            _optionService = optionService;
            _amazonOrderSearchService = amazonOrderSearchService;
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
            ViewData["ShippingStatuses"] = _optionService.GetEnumOptions<ShippingStatus>();
            model.Results = new PagedList<AmazonOrder>(null, 1, 10);
            try
            {
                model.Results = _amazonOrderSearchService.Search(model.Email, model.Name, model.AmazonOrderId,
                    model.DateFrom.HasValue ? model.DateFrom.Value : DateTime.Now, model.DateTo.HasValue ? model.DateTo.Value : DateTime.Now,
                    model.ShippingStatus, model.Page);
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
            if(amazonOrder!=null)
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
                return View(new AmazonSyncModel() { Id = amazonOrder.Id, Description = amazonOrder.AmazonOrderId});
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Sync(AmazonSyncModel model)
        {
            if (model != null)
            {
                _syncAmazonOrderService.SyncOrders(model);
                return Json(true);
            }
            return Json(false);
        }
    }
}
