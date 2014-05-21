
using System;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Misc;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderService _orderService;
        private readonly IOptionService _optionService;
        private readonly SiteSettings _ecommerceSettings;
        private readonly IExportOrdersService _exportOrdersService;
        private readonly IUserService _userService;
        private readonly IShippingMethodManager _shippingMethodManager;
        private readonly IOrderSearchService _orderSearchService;
        private readonly IOrderShippingService _orderShippingService;

        public OrderController(IOrderService orderService,  IShippingMethodManager shippingMethodManager,
            IOrderSearchService orderSearchService, IOrderShippingService orderShippingService,
            IOptionService optionService, SiteSettings ecommerceSettings, IExportOrdersService exportOrdersService, IUserService userService)
        {
            _orderService = orderService;
            _shippingMethodManager = shippingMethodManager;
            _orderSearchService = orderSearchService;
            _orderShippingService = orderShippingService;
            _optionService = optionService;
            _ecommerceSettings = ecommerceSettings;
            _exportOrdersService = exportOrdersService;
            _userService = userService;
        }

        [HttpGet]
        public ViewResult Index(OrderSearchModel model, int page = 1)
        {
            ViewData["ShippingStatuses"] = GeneralHelper.GetEnumOptionsWithEmpty<ShippingStatus>();
            ViewData["PaymentStatuses"] = GeneralHelper.GetEnumOptionsWithEmpty<PaymentStatus>();
            model.Results = new PagedList<Order>(null, 1, _ecommerceSettings.DefaultPageSize);
            try
            {
                model.Results = _orderSearchService.SearchOrders(model, page, _ecommerceSettings.DefaultPageSize);
            }
            catch (Exception)
            {
                model.Results = _orderService.GetPaged(page);
            }

            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Edit(Order order)
        {
            ViewData["ShippingStatuses"] = _optionService.GetEnumOptions<ShippingStatus>();
            ViewData["PaymentStatuses"] = _optionService.GetEnumOptions<PaymentStatus>();
            ViewData["ShippingMethods"] = _shippingMethodManager.GetOptions();
            if (order.ShippingMethod != null)
                ViewData["ShippingMethodId"] = order.ShippingMethod.Id;
            return order != null
                       ? (ActionResult)View(order)
                       : RedirectToAction("Index");
        }

        [ActionName("Edit")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Edit_POST(Order order, int shippingMethodId = 0)
        {
            if (shippingMethodId != 0)
                order.ShippingMethod = _shippingMethodManager.Get(shippingMethodId);
            order.User = CurrentRequestData.CurrentUser;
            _orderService.Save(order);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Cancel(Order order)
        {
            return PartialView(order);
        }

        [ActionName("Cancel")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Cancel_POST(Order order)
        {
            _orderService.Cancel(order);
            return RedirectToAction("Edit", "Order", new {id = order.Id});
        }

        [HttpGet]
        public PartialViewResult MarkAsShipped(Order order, bool index = false)
        {
            ViewBag.Index = index;
            var selectedShippingMethod = order.ShippingMethod != null ? order.ShippingMethod.Id : 0;

            ViewData["ShippingMethods"] = _shippingMethodManager.GetAll().BuildSelectItemList(
                method => method.Name,
                method => method.Id.ToString(),
                method => method.Id.ToString() == selectedShippingMethod.ToString(),
                emptyItem: null);

            return PartialView(order);
        }

        [ActionName("MarkAsShipped")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult MarkAsShipped_POST(Order order,bool index = false)
        {
            _orderService.MarkAsShipped(order);
            return !index ? RedirectToAction("Edit", "Order", new { id = order.Id }) : RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult MarkAsPaid(Order order, bool index = false)
        {
            ViewBag.Index = index;
            return PartialView(order);
        }

        [ActionName("MarkAsPaid")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult MarkAsPaid_POST(Order order, bool index = false)
        {
            _orderService.MarkAsPaid(order);
            return !index ? RedirectToAction("Edit", "Order", new { id = order.Id }) : RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult MarkAsVoided(Order order, bool index = false)
        {
            ViewBag.Index = index;
            return PartialView(order);
        }

        [ActionName("MarkAsVoided")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult MarkAsVoided_POST(Order order, bool index = false)
        {
            _orderService.MarkAsVoided(order);
            return !index ? RedirectToAction("Edit", "Order", new { id = order.Id }) : RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult BulkShippingUpdate()
        {
            if (TempData.ContainsKey("messages"))
                ViewBag.Messages = TempData["messages"];
            if (TempData.ContainsKey("import-status"))
                ViewBag.ImportStatus = TempData["import-status"];
            return View();
        }

        [HttpPost]
        [ActionName("BulkShippingUpdate")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult BulkShippingUpdate_POST(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0 && (document.ContentType.ToLower() == "text/csv" || document.ContentType.ToLower().Contains("excel")))
                TempData["messages"] = _orderShippingService.BulkShippingUpdate(document.InputStream);
            else
                TempData["import-status"] = "Please choose non-empty CSV (.csv) file before uploading.";
            return RedirectToAction("BulkShippingUpdate");
        }

        [HttpGet]
        public ActionResult SetTrackingNumber(Order order)
        {
            return View(order);
        }

        [HttpPost]
        [ActionName("SetTrackingNumber")]
        [ForceImmediateLuceneUpdate]
        public ActionResult SetTrackingNumber_POST(Order order)
        {
            _orderService.Save(order);
            return RedirectToAction("Edit", "Order", new {id = order.Id});
        }

        [HttpGet]
        public ActionResult Delete(Order order)
        {
            return View(order);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        public ActionResult Delete_POST(Order order)
        {
            _orderService.Delete(order);
            return RedirectToAction("Index", "Order");
        }

        [HttpGet]
        public ActionResult ExportOrderToPdf(Order order)
        {
            try
            {
                var file = _exportOrdersService.ExportOrderToPdf(order);
                return File(file, "application/pdf", "Order-" + order.Id + "-[" + CurrentRequestData.Now.ToString("dd-MM-yyyy hh-mm") + "].pdf");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            }
        }

        [ChildActionOnly]
        public PartialViewResult ForUser(User user)
        {
            var orders = _userService.GetAll<Order>(user);
            return PartialView(orders);
        }
    }
}