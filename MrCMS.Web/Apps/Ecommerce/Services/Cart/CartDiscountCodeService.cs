using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartDiscountCodeService : ICartDiscountCodeService, ICartSessionKeyList
    {
        public const string CurrentDiscountCodesKey = "current.discount-codes";
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;

        public CartDiscountCodeService(ICartSessionManager cartSessionManager, IGetUserGuid getUserGuid)
        {
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
        }

        public HashSet<string> Get(Guid? guid = null)
        {
            return _cartSessionManager.GetSessionValue(CurrentDiscountCodesKey, guid ?? _getUserGuid.UserGuid, new HashSet<string>());
        }

        public void SaveDiscounts(HashSet<string> codes)
        {
            _cartSessionManager.SetSessionValue(CurrentDiscountCodesKey, _getUserGuid.UserGuid, codes);
        }

        public IEnumerable<string> Keys
        {
            get { yield return CurrentDiscountCodesKey; }
        }
    }
}