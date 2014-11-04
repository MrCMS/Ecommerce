using MrCMS.Web.Apps.Ecommerce.Services.Cart;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IGiftMessageUIService
    {
        void Save(string message);
    }

    public class GiftMessageUIService : IGiftMessageUIService
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;

        public GiftMessageUIService(ICartSessionManager cartSessionManager,IGetUserGuid getUserGuid)
        {
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
        }

        public void Save(string message)
        {
            _cartSessionManager.SetSessionValue(CartManager.CurrentGiftMessageKey, _getUserGuid.UserGuid, message);
        }
    }
}