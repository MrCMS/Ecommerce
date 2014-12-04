using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartDiscountService : ICartDiscountService, ICartSessionKeyList
    {
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetDiscountCodes _getDiscountCodes;
        private readonly IGetUserGuid _getUserGuid;

        public CartDiscountService(ICartSessionManager cartSessionManager, IGetDiscountCodes getDiscountCodes, IGetUserGuid getUserGuid)
        {
            _cartSessionManager = cartSessionManager;
            _getDiscountCodes = getDiscountCodes;
            _getUserGuid = getUserGuid;
        }

        public const string CurrentDiscountCodesKey = "current.discount-codes";
        public void AddDiscountCode(string code)
        {
            var codes = _getDiscountCodes.Get(_getUserGuid.UserGuid);
            codes.Add(code);
            _cartSessionManager.SetSessionValue(CurrentDiscountCodesKey, _getUserGuid.UserGuid, codes);
        }

        public void RemoveDiscountCode(string discountCode)
        {
            var codes = _getDiscountCodes.Get(_getUserGuid.UserGuid);
            codes.Remove(discountCode);
            _cartSessionManager.SetSessionValue(CurrentDiscountCodesKey, _getUserGuid.UserGuid, codes);
        }

        public IEnumerable<string> Keys { get { yield return CurrentDiscountCodesKey; } }
    }
}