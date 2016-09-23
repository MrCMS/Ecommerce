using System;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.ProductReviews;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IOrderService _orderService;
        private readonly IUserLookup _userLookup;
        private readonly IUserManagementService _userManagementService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IAuthorisationService _authorisationService;
        private readonly IProductReviewUIService _productReviewUIService;

        public UserAccountController(IOrderService orderService,
            IUserLookup userLookup, 
            IPasswordManagementService passwordManagementService, 
            IAuthorisationService authorisationService, IProductReviewUIService productReviewUIService, IUserManagementService userManagementService)
        {
            _orderService = orderService;
            _userLookup = userLookup;
            _passwordManagementService = passwordManagementService;
            _authorisationService = authorisationService;
            _productReviewUIService = productReviewUIService;
            _userManagementService = userManagementService;
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

        public ActionResult UserAccountOrder(UserOrder page, Guid id)
        {
            var user = CurrentRequestData.CurrentUser;
            if (user != null)
            {
                var order = _orderService.GetByGuid(id);

                if (order.User.Id != user.Id)
                {
                    return Redirect(UniquePageHelper.GetUrl<LoginPage>());
                }
                ViewData["Order"] = order;
                return View(page);
            }
            return Redirect(UniquePageHelper.GetUrl<LoginPage>());
        }

        public ActionResult UserAccountReviews(int page = 1)
        {
            var user = CurrentRequestData.CurrentUser;
            if (user != null)
            {
                var reviewsByUser = _productReviewUIService.GetReviewsByUser(user, page);

                var model = new UserAccountReviewsModel(new PagedList<ProductReview>(reviewsByUser, reviewsByUser.PageNumber, reviewsByUser.PageSize), user.Id);
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
                var existingUser = _userLookup.GetUserByEmail(model.Email);
                if (existingUser != null)
                    return Redirect(UniquePageHelper.GetUrl<ProductSearch>());
                
                
                var user = new User
                {
                    FirstName = String.Empty,
                    LastName = String.Empty,
                    Email = model.Email.Trim(),
                    IsActive = true
                };

                _passwordManagementService.SetPassword(user, model.Password, model.Password);
                _userManagementService.AddUser(user);
                _authorisationService.SetAuthCookie(user, false);
                CurrentRequestData.CurrentUser = user;

                var order = _orderService.AssignUserToOrder(model.OrderId, user);
                if (order.BillingAddress != null)
                {
                    user.FirstName = order.BillingAddress.FirstName;
                    user.LastName = order.BillingAddress.LastName;
                    _userManagementService.SaveUser(user);
                }

                return Redirect(UniquePageHelper.GetUrl<UserAccountPage>());
            }
            return Redirect(UniquePageHelper.GetUrl<RegisterPage>());
        }
    }
}