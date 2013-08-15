using System;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaypointController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IPaypointPaymentService _paypointPaymentService;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IDocumentService _documentService;
        private readonly CartModel _cartModel;
        private readonly IOrderService _orderService;

        public PaypointController(IPaypointPaymentService paypointPaymentService, ICartSessionManager cartSessionManager, IDocumentService documentService, CartModel cartModel, IOrderService orderService)
        {
            _paypointPaymentService = paypointPaymentService;
            _cartSessionManager = cartSessionManager;
            _documentService = documentService;
            _cartModel = cartModel;
            _orderService = orderService;
        }

        public PartialViewResult PaymentDetails(PaypointPaymentDetailsModel model)
        {
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult PostDetails(PaypointPaymentDetailsModel model)
        {
            var response = _paypointPaymentService.ProcessDetails(model, Url.Action("Response3DSecure", "Paypoint"));
            if (response.Requires3DSecure)
            {
                TempData["redirect-details"] = response.RedirectDetails;
                return RedirectToAction("Redirect3DSecure");
            }
            else if (response.PaymentSucceeded)
            {
                var order = _orderService.PlaceOrder(_cartModel, o => { });
                _documentService.RedirectTo<OrderPlaced>(new { id = order.Guid });
            }
            else
            {
                TempData["error-details"] = response.FailureDetails;
                return _documentService.RedirectTo<PaymentDetails>();
            }


            throw new NotImplementedException();
        }

        public ActionResult Response3DSecure(FormCollection formCollection)
        {
            throw new NotImplementedException();
        }

        public ActionResult Redirect3DSecure()
        {
            var redirectDetails = TempData["redirect-details"] as RedirectDetails;
            if (redirectDetails != null)
                return View(redirectDetails);
            return _documentService.RedirectTo<PaymentDetails>();
        }
    }
}