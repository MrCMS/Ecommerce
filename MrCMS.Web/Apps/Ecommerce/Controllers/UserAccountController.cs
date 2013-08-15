using System;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IAuthorisationService _authorisationService;

        public UserAccountController(IOrderService orderService,
            IUserService userService, 
            IPasswordManagementService passwordManagementService, 
            IAuthorisationService authorisationService)
        {
            _orderService = orderService;
            _userService = userService;
            _passwordManagementService = passwordManagementService;
            _authorisationService = authorisationService;
        }

        public ActionResult UserAccountOrders(int page = 1)
        {
            var user = CurrentRequestData.CurrentUser;
            if (user != null)
            {
                var ordersByUser = _orderService.GetOrdersByUser(user, page);
                var model = new UserAccountOrdersModel(new PagedList<Order>(ordersByUser, ordersByUser.PageNumber, ordersByUser.PageSize), user.Id);
                return View(model);
            }
            return Redirect(UniquePageHelper.GetUrl<LoginPage>());
        }

        [HttpPost]
        public ActionResult RegistrationWithoutDetails(RegisterWithoutDetailsModel model)
        {
            if (CurrentRequestData.CurrentUser != null)
            {
                return Redirect(UniquePageHelper.GetUrl<ProductSearch>());
            }

            if (model != null && ModelState.IsValid)
            {
                var user = new User
                {
                    FirstName = String.Empty,
                    LastName = String.Empty,
                    Email = model.Email,
                    IsActive = true
                };
                _passwordManagementService.SetPassword(user, model.Password, model.Password);
                _userService.AddUser(user);
                _authorisationService.SetAuthCookie(model.Email, false);
                CurrentRequestData.CurrentUser = user;

                _orderService.SetLastOrderUserIdByOrderEmail(model.Email);

                return Redirect(UniquePageHelper.GetUrl<UserAccountPage>());
            }
            return Redirect(UniquePageHelper.GetUrl<RegisterPage>());
        }
    }
}