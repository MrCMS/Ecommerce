using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IUserService _userService;
        private readonly IAuthorisationService _authorisationService;
        private readonly IOrderService _orderService;

        public UserAccountController(IUserService userService, IAuthorisationService authorisationService, IOrderService orderService)
        {
            _userService = userService;
            _authorisationService = authorisationService;
            _orderService = orderService;
        }

        public ActionResult UserAccount(UserAccount page, int pageNum=1)
        {
            if (CurrentRequestData.CurrentUser != null)
            {
                ViewBag.PageNum = pageNum;
                return View(page);
            }
            return Redirect(UniquePageHelper.GetUrl<UserLogin>());
        }

        [HttpGet]
        public ActionResult UserAccountDetails(UserAccountModel model)
        {
            if (CurrentRequestData.CurrentUser != null)
            {
                model.FirstName = CurrentRequestData.CurrentUser.FirstName;
                model.LastName = CurrentRequestData.CurrentUser.LastName;
                model.Email = CurrentRequestData.CurrentUser.Email;
                return View(model);
            }

            return Redirect(UniquePageHelper.GetUrl<UserLogin>());
        }

        [HttpGet]
        public ActionResult UserAccountOrders(int pageNum=1)
        {
            if (CurrentRequestData.CurrentUser != null)
                return View(_orderService.GetOrdersByUser(CurrentRequestData.CurrentUser, pageNum, 10));
            return Redirect(UniquePageHelper.GetUrl<UserLogin>());
        }

        [HttpPost]
        public ActionResult UpdateAccount(UserAccountModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var user = CurrentRequestData.CurrentUser;
                if (user != null && user.IsActive)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    if(!string.IsNullOrWhiteSpace(model.Password) && 
                        !string.IsNullOrWhiteSpace(model.ConfirmPassword) &&
                        model.Password==model.ConfirmPassword)
                        _authorisationService.SetPassword(user,model.Password,model.ConfirmPassword);
                    _userService.SaveUser(user);

                    return Redirect(UniquePageHelper.GetUrl<UserAccount>());
                }
            }
            return Redirect(UniquePageHelper.GetUrl<UserAccount>());
        }
    }
}