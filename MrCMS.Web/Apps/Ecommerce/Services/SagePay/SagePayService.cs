using System;
using System.Collections.Generic;
using System.Web.Routing;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public class SagePayService : ISagePayService, ICartSessionKeyList
    {
        private readonly ITransactionRegistrar _transactionRegistrar;
        private readonly ICartSessionManager _cartSessionManager;
        private readonly IGetUserGuid _getUserGuid;
        private const string SagePayEnrolledResponseKey = "current.sagepayenrolledresponse";
        private const string SagePayTransactionResponseKey = "current.sagepaytransactionresponse";

        public SagePayService(ITransactionRegistrar transactionRegistrar, ICartSessionManager cartSessionManager, IGetUserGuid getUserGuid)
        {
            _transactionRegistrar = transactionRegistrar;
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
        }

        public TransactionRegistrationResponse RegisterTransaction(CartModel model)
        {
            var response = _cartSessionManager.GetSessionValue<TransactionRegistrationResponse>(SagePayEnrolledResponseKey, _getUserGuid.UserGuid);
            if (response != null)
                return response;

            var transactionRegistrationResponse = _transactionRegistrar.Send(model);

            if (transactionRegistrationResponse.Status == ResponseType.Ok)
                _cartSessionManager.SetSessionValue(SagePayEnrolledResponseKey, _getUserGuid.UserGuid, transactionRegistrationResponse);

            return transactionRegistrationResponse;
        }

        public string GetSecurityKey(Guid userGuid)
        {
            var response = _cartSessionManager.GetSessionValue<TransactionRegistrationResponse>(SagePayEnrolledResponseKey, userGuid);
            return response != null ? response.SecurityKey : null;
        }

        public void SetResponse(Guid userGuid, SagePayResponse response)
        {
            _cartSessionManager.SetSessionValue(SagePayTransactionResponseKey, userGuid, response, true);
        }
        
        public SagePayResponse GetResponse(Guid userGuid)
        {
            return _cartSessionManager.GetSessionValue<SagePayResponse>(SagePayTransactionResponseKey, userGuid,
                                                                        encrypted: true);
        }

        public IEnumerable<string> Keys { get { yield return SagePayEnrolledResponseKey; } }
    }
}