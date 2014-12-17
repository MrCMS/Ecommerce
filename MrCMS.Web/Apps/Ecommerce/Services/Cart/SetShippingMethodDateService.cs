using System;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class SetShippingMethodDateService : ISetShippingMethodDateService
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;
        private readonly IUniquePageService _uniquePageService;

        public SetShippingMethodDateService(ICartSessionManager cartSessionManager,IGetUserGuid getUserGuid,IUniquePageService uniquePageService)
        {
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
            _uniquePageService = uniquePageService;
        }

        public void SetDate(DateTime date)
        {
            _cartSessionManager.SetSessionValue(CartManager.CurrentShippingDateKey, _getUserGuid.UserGuid, date.Date);
        }

        public ActionResult RedirectToSetShippingDetails()
        {
            return _uniquePageService.RedirectTo<SetShippingDetails>();
        }
    }
}