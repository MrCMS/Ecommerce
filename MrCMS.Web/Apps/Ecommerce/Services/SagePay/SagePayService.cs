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
        private readonly ICartGuidResetter _cartGuidResetter;
        private const string SagePayEnrolledResponseKey = "current.sagepayenrolledresponse";
        private const string SagePayTransactionResponseKey = "current.sagepaytransactionresponse";
        private const string SagePayFailureDetailsKey = "current.sagepayfailuredetails";

        public SagePayService(ITransactionRegistrar transactionRegistrar, ICartSessionManager cartSessionManager, IGetUserGuid getUserGuid, ICartGuidResetter cartGuidResetter)
        {
            _transactionRegistrar = transactionRegistrar;
            _cartSessionManager = cartSessionManager;
            _getUserGuid = getUserGuid;
            _cartGuidResetter = cartGuidResetter;
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

        public void ResetSessionInfo(Guid userGuid)
        {
            _cartGuidResetter.ResetCartGuid(userGuid);
            foreach (var key in Keys)
                _cartSessionManager.RemoveValue(key, userGuid);
        }

        public void SetFailureDetails(Guid userGuid, FailureDetails failureDetails)
        {
            _cartSessionManager.SetSessionValue(SagePayFailureDetailsKey, userGuid, failureDetails);
        }

        public FailureDetails GetFailureDetails(Guid userGuid)
        {
            var details = _cartSessionManager.GetSessionValue<FailureDetails>(SagePayFailureDetailsKey, userGuid);
            _cartSessionManager.RemoveValue(SagePayFailureDetailsKey, userGuid);
            return details;
        }

        public IEnumerable<string> Keys
        {
            get
            {
                yield return SagePayEnrolledResponseKey;
                yield return SagePayTransactionResponseKey;
                yield return SagePayFailureDetailsKey;
            }
        }
    }
}