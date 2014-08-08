using System;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartBuilder : ICartBuilder
    {
        private readonly IAssignBasicCartInfo _assignBasicCartInfo;
        private readonly IAssignCartDiscountInfo _assignCartDiscountInfo;
        private readonly IAssignPaymentInfo _assignPaymentInfo;
        private readonly IAssignShippingInfo _assignShippingInfo;
        private readonly IGetUserGuid _getUserGuid;

        public CartBuilder(IAssignBasicCartInfo assignBasicCartInfo,
            IAssignCartDiscountInfo assignCartDiscountInfo,
            IAssignShippingInfo assignShippingInfo,
            IAssignPaymentInfo assignPaymentInfo,
            IGetUserGuid getUserGuid)
        {
            _assignBasicCartInfo = assignBasicCartInfo;
            _assignCartDiscountInfo = assignCartDiscountInfo;
            _assignShippingInfo = assignShippingInfo;
            _assignPaymentInfo = assignPaymentInfo;
            _getUserGuid = getUserGuid;
        }

        public CartModel BuildCart()
        {
            return BuildCart(_getUserGuid.UserGuid);
        }

        public CartModel BuildCart(Guid userGuid)
        {
            var cart = new CartModel();

            cart = _assignBasicCartInfo.Assign(cart, userGuid);
            cart = _assignCartDiscountInfo.Assign(cart, userGuid);
            cart = _assignShippingInfo.Assign(cart, userGuid);
            cart = _assignPaymentInfo.Assign(cart, userGuid);
            return cart;
        }
    }
}