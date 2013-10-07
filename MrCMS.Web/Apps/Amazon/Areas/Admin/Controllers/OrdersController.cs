using System;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
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
        private readonly IAmazonOrderSearchService _amazonOrderSearchService;
        private readonly EcommerceSettings _ecommerceSettings;

        public OrdersController(IAmazonOrderSyncManager syncAmazonOrderService,
            IAmazonOrderService amazonOrderService, AmazonAppSettings amazonAppSettings, 
            IAmazonOrderSearchService amazonOrderSearchService, EcommerceSettings ecommerceSettings)
        {
            _syncAmazonOrderService = syncAmazonOrderService;
            _amazonOrderService = amazonOrderService;
            _amazonAppSettings = amazonAppSettings;
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
    }
}
