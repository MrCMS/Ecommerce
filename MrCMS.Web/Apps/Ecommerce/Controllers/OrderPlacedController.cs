using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class OrderPlacedController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IOrderPlacedService _orderPlacedService;
        private readonly IDocumentService _documentService;

        public OrderPlacedController(IOrderPlacedService orderPlacedService,IDocumentService documentService)
        {
            _orderPlacedService = orderPlacedService;
            _documentService = documentService;
        }

        public ActionResult Show(OrderPlaced page, [IoCModelBinder(typeof(OrderByGuidModelBinder))]Order order)
        {
            if (order != null)
            {
                ViewData["order"] = order;
                TempData["order"] = order;//required for Google Analytics

                ViewData["user-can-register"] = _orderPlacedService.GetRegistrationStatus(order.OrderEmail);

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
            return _documentService.RedirectTo<OrderPlaced>(new {id = order.Guid});
        }

        [HttpPost]
        public RedirectResult RegisterAndAssociateOrder(RegisterModel model, [IoCModelBinder(typeof(OrderByGuidModelBinder))]Order order)
        {
            var result = _orderPlacedService.RegisterAndAssociateOrder(model, order);
            if (!result.Success)
                TempData["register-error"] = result.Error;
            return _documentService.RedirectTo<OrderPlaced>(new {id = order.Guid});
        }
    }

    public class OrderPlacedLoginModel : LoginModel
    {
        public Order Order { get; set; }
    }

    public interface IOrderPlacedService
    {
        EmailRegistrationStatus GetRegistrationStatus(string orderEmail);
        LoginAndAssociateOrderResult LoginAndAssociateOrder(LoginModel model, Order order);
        RegisterAndAssociateOrderResult RegisterAndAssociateOrder(RegisterModel model, Order order);
    }

    public class LoginAndAssociateOrderResult
    {
        public string Error { get; set; }
        public bool Success { get { return string.IsNullOrWhiteSpace(Error); } }
    }

    public class RegisterAndAssociateOrderResult
    {
        public string Error { get; set; }
        public bool Success { get { return string.IsNullOrWhiteSpace(Error); } }
    }

    public class OrderPlacedService : IOrderPlacedService
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly ISession _session;
        private readonly IRegistrationService _registrationService;

        public OrderPlacedService(IUserService userService, ILoginService loginService, ISession session,IRegistrationService registrationService)
        {
            _userService = userService;
            _loginService = loginService;
            _session = session;
            _registrationService = registrationService;
        }

        public EmailRegistrationStatus GetRegistrationStatus(string orderEmail)
        {
            if (CurrentRequestData.CurrentUser != null)
                return EmailRegistrationStatus.LoggedIn;
            if (_userService.GetUserByEmail(orderEmail) != null)
                return EmailRegistrationStatus.EmailInUse;
            return EmailRegistrationStatus.Available;
        }

        public LoginAndAssociateOrderResult LoginAndAssociateOrder(LoginModel model, Order order)
        {
            var authenticateUser = _loginService.AuthenticateUser(model);
            if (!authenticateUser.Success)
                return new LoginAndAssociateOrderResult { Error = "We were unable to log you in, please check your password and try again" };

            order.User = CurrentRequestData.CurrentUser;
            _session.Transact(session => session.Update(order));
            return new LoginAndAssociateOrderResult();
        }

        public RegisterAndAssociateOrderResult RegisterAndAssociateOrder(RegisterModel model, Order order)
        {
            if (!_registrationService.CheckEmailIsNotRegistered(model.Email))
                return new RegisterAndAssociateOrderResult { Error = "The provided email already has an account associated" };

            var registeredUser = _registrationService.RegisterUser(model);
            order.User = registeredUser;
            _session.Transact(session => session.Update(order));
            return new RegisterAndAssociateOrderResult();
        }
    }

    public enum EmailRegistrationStatus
    {
        LoggedIn,
        EmailInUse,
        Available
    }
}