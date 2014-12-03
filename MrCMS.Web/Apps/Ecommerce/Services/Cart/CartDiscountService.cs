using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartDiscountService : ICartDiscountService, ICartSessionKeyList
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;

        public CartDiscountService(ICartSessionManager cartSessionManager, IGetUserGuid getUserGuid)
        {
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
        }

        public const string CurrentDiscountCodesKey = "current.discount-codes";
        public void SetDiscountCode(string code)
        {
            _cartSessionManager.SetSessionValue(CurrentDiscountCodesKey, _getUserGuid.UserGuid, code);
        }

        public IEnumerable<string> Keys { get { yield return CurrentDiscountCodesKey; } }
    }
}