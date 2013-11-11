using System.Web.Routing;
using MrCMS.Web.Apps.Ecommerce.Models;
using SagePayMvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public class SagePayService : ISagePayService
    {
        private readonly RequestContext _requestContext;
        private readonly ITransactionRegistrar _transactionRegistrar;
        private readonly ISagePayItemCreator _sagePayItemCreator;

        public SagePayService(RequestContext requestContext, ITransactionRegistrar transactionRegistrar, ISagePayItemCreator sagePayItemCreator)
        {
            _requestContext = requestContext;
            _transactionRegistrar = transactionRegistrar;
            _sagePayItemCreator = sagePayItemCreator;
        }

        public TransactionRegistrationResponse RegisterTransaction(CartModel model)
        {
            return _transactionRegistrar.Send(_requestContext, model.CartGuid.ToString(),
                                              _sagePayItemCreator.GetShoppingBasket(model),
                                              _sagePayItemCreator.GetAddress(model.BillingAddress),
                                              _sagePayItemCreator.GetAddress(model.ShippingAddress),
                                              model.OrderEmail, PaymentFormProfile.Low);
        }
    }
}