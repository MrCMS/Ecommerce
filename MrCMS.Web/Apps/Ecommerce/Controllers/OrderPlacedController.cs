using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class OrderPlacedController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IOrderPlacedService _orderPlacedService;
        private readonly IUniquePageService _uniquePageService;

        public OrderPlacedController(IOrderPlacedService orderPlacedService, IUniquePageService uniquePageService)
        {
            _orderPlacedService = orderPlacedService;
            _uniquePageService = uniquePageService;
        }

        public ActionResult Show(OrderPlaced page, [IoCModelBinder(typeof(OrderByGuidModelBinder))]Order order)
        {
            if (order != null)
            {
                ViewData["order"] = order;
                ViewData["user-can-register"] = _orderPlacedService.GetRegistrationStatus(order.OrderEmail);
                ViewData["render-analytics"] = _orderPlacedService.UpdateAnalytics(order);

                return View(page);
            }
            return Redirect(UniquePageHelper.GetUrl<ProductSearch>());
        }

        [HttpPost]
        public RedirectResult LoginAndAssociateOrder(LoginModel model, [IoCModelBinder(typeof(OrderByGuidModelBinder))]Order order)
        {
            var result = _orderPlacedService.LoginAndAssociateOrder(model, order);
            if (!result.Success)
                TempData["login-error"] = result.Error;
            return _uniquePageService.RedirectTo<OrderPlaced>(new { id = order.Guid });
        }

        [HttpPost]
        public async Task<RedirectResult> RegisterAndAssociateOrder(RegisterModel model, [IoCModelBinder(typeof(OrderByGuidModelBinder))]Order order)
        {
            model.ConfirmPassword = model.Password; // RegisterationService RegisterUser requires ConfirmPassword which our form doesn't contain;
            var result = await _orderPlacedService.RegisterAndAssociateOrder(model, order);
            if (!result.Success)
                TempData["register-error"] = result.Error;
            return _uniquePageService.RedirectTo<OrderPlaced>(new { id = order.Guid });
        }
    }
}