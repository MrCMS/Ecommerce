using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetDiscountCodes : IGetDiscountCodes
    {
        public GetDiscountCodes(ICartSessionManager cartSessionManager)
        {
            _cartSessionManager = cartSessionManager;
        }

        private readonly ICartSessionManager _cartSessionManager;
        
        public List<string> Get(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue(CartDiscountService.CurrentDiscountCodesKey,
                userGuid, new List<string>());
        }
    }
}