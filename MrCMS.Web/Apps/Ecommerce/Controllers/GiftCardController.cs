using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class GiftCardController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGiftCardUIService _giftCardUIService;

        public GiftCardController(IGiftCardUIService giftCardUIService)
        {
            _giftCardUIService = giftCardUIService;
        }

        [HttpPost]
        public JsonResult Apply(string giftCardCode)
        {
            return Json(_giftCardUIService.Apply(giftCardCode));
        }

        [HttpPost]
        public JsonResult Remove(string giftCardCode)
        {
            return Json(_giftCardUIService.Remove(giftCardCode));
        }
    }

    public interface IGiftCardUIService
    {
        GiftCardUIServiceResult Apply(string giftCardCode);
        GiftCardUIServiceResult Remove(string giftCardCode);
    }

    public class GiftCardUIServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class GiftCardUIService : IGiftCardUIService
    {
        private readonly ISession _session;
        private readonly ICartManager _cartManager;
        private readonly CartModel _cart;
        private readonly IStringResourceProvider _stringResourceProvider;

        public GiftCardUIService(ISession session, ICartManager cartManager, CartModel cart,
            IStringResourceProvider stringResourceProvider)
        {
            _session = session;
            _cartManager = cartManager;
            _cart = cart;
            _stringResourceProvider = stringResourceProvider;
        }

        public GiftCardUIServiceResult Apply(string giftCardCode)
        {
            giftCardCode = giftCardCode.Trim();
            var giftCard =
                _session.QueryOver<GiftCard>()
                    .Where(card => card.Code == giftCardCode)
                    .Cacheable()
                    .List().FirstOrDefault(card => card.IsValidToUse());
            if (giftCard != null)
            {
                if (_cart.AppliedGiftCards.Contains(giftCard))
                {
                    return new GiftCardUIServiceResult
                    {
                        Success = false,
                        Message = _stringResourceProvider.GetValue("Gift card already applied")
                    };
                }
                _cartManager.AddGiftCard(giftCard.Code);
                return new GiftCardUIServiceResult
                {
                    Success = true,
                    Message = _stringResourceProvider.GetValue("Gift card applied")
                };
            }
            return new GiftCardUIServiceResult
            {
                Success = false,
                Message = _stringResourceProvider.GetValue("Gift card code could not be applied")
            };
        }

        public GiftCardUIServiceResult Remove(string giftCardCode)
        {
                _cartManager.RemoveGiftCard(giftCardCode);
                return new GiftCardUIServiceResult
                {
                    Success = true,
                    Message = _stringResourceProvider.GetValue("Gift card removed")
                };
        }
    }
}