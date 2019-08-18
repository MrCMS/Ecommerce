using System;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OnlineCustomersController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOnlineCustomerService _onlineCustomerService;

        public OnlineCustomersController(IOnlineCustomerService onlineCustomerService)
        {
            _onlineCustomerService = onlineCustomerService;
        }

        [MrCMSACLRule(typeof(OnlineCustomersACL), OnlineCustomersACL.ViewCustomers)]
        public ViewResult Index(OnlineCustomerSearchQuery searchQuery)
        {
            ViewData["results"] = _onlineCustomerService.Search(searchQuery);

            return View(searchQuery);
        }

        [MrCMSACLRule(typeof(OnlineCustomersACL), OnlineCustomersACL.ViewCart)]
        public ViewResult Cart(Guid id)
        {
            var cartItems = _onlineCustomerService.GetCart(id);

            return View(cartItems);
        }
    }
}