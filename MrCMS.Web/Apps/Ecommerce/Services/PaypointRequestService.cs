using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using MrCMS.PaypointService.API;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Paypoint;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class PaypointRequestService : IPaypointRequestService
    {
        private readonly SECVPN _secvpn;
        private readonly CartModel _cartModel;
        private readonly IPaypointRequestHelper _paypointHelper;
        private readonly PaypointSettings _paypointSettings;

        public PaypointRequestService(SECVPN secvpn, CartModel cartModel, IPaypointRequestHelper paypointHelper, PaypointSettings paypointSettings)
        {
            _secvpn = secvpn;
            _cartModel = cartModel;
            _paypointHelper = paypointHelper;
            _paypointSettings = paypointSettings;
        }

        public ProcessDetailsResponse ProcessStandardTransaction(PaypointPaymentDetailsModel model)
        {
            var validateCardFullResponse =
                _secvpn.validateCardFull(new validateCardFullRequest(_paypointSettings.AccountName,
                                                                     _paypointSettings.VPNPassword,
                                                                     _cartModel.CartGuid.ToString(),
                                                                     RequestHelper.GetIP(), model.NameOnCard,
                                                                     model.CardNumber,
                                                                     _paypointHelper.GetTotal(_cartModel.Total),
                                                                     _paypointHelper.GetDate(model.EndMonth,
                                                                                             model.EndYear),
                                                                     model.CardIssueNumber,
                                                                     _paypointHelper.GetDate(model.StartMonth,
                                                                                             model.StartYear),
                                                                     _paypointHelper.GetOrderDetails(_cartModel),
                                                                     _paypointHelper.GetAddress(
                                                                         _cartModel.ShippingAddress,
                                                                         _cartModel.OrderEmail),
                                                                     _paypointHelper.GetAddress(
                                                                         _cartModel.BillingAddress,
                                                                         _cartModel.OrderEmail),
                                                                     _paypointHelper.GetOptions(model)));

            var response = _paypointHelper.ParseEnrolmentResponse(validateCardFullResponse.validateCardFullReturn);
            return response["valid"] != "true"
                       ? GetFailureResponse(response)
                       : GetSuccessResponse(response);
        }

        private static ProcessDetailsResponse GetSuccessResponse(NameValueCollection response)
        {
            return new ProcessDetailsResponse
                       {
                           PaypointPaymentDetails = new PaypointPaymentDetails
                                                        {
                                                            TransactionId = response["trans_id"],
                                                            AuthCode = response["auth_code"],
                                                        }
                       };
        }

        public ProcessDetailsResponse Process3DSecureTransaction(PaypointPaymentDetailsModel model, string threeDSecureUrl)
        {
            var threeDSecureEnrolmentRequestResponse =
                _secvpn.threeDSecureEnrolmentRequest(
                    new threeDSecureEnrolmentRequestRequest(_paypointSettings.AccountName,
                                                            _paypointSettings.VPNPassword,
                                                            _cartModel.CartGuid.ToString(),
                                                            RequestHelper.GetIP(),
                                                            model.NameOnCard, model.CardNumber,
                                                            _paypointHelper.GetTotal(_cartModel.Total),
                                                            _paypointHelper.GetDate(model.EndMonth,
                                                                                    model.EndYear),
                                                            model.CardIssueNumber,
                                                            _paypointHelper.GetDate(model.StartMonth,
                                                                                    model.StartYear),
                                                            _paypointHelper.GetOrderDetails(_cartModel),
                                                            _paypointHelper.GetAddress(
                                                                _cartModel.ShippingAddress,
                                                                _cartModel.OrderEmail),
                                                            _paypointHelper.GetAddress(
                                                                _cartModel.BillingAddress, _cartModel.OrderEmail),
                                                            _paypointHelper.GetOptions(model), "0",
                                                            RequestHelper.GetAcceptHeaders(),
                                                            RequestHelper.UserAgent(),
                                                            _paypointSettings.MPIMerchantName,
                                                            _paypointSettings.MPIMerchantUrl,
                                                            _paypointSettings.MPIDescription, "", "", ""));

            var response = _paypointHelper.ParseEnrolmentResponse(threeDSecureEnrolmentRequestResponse.threeDSecureEnrolmentRequestReturn);

            return response["valid"] != "true"
                       ? GetFailureResponse(response)
                       : (response["mpi_status_code"] == "200"
                              ? GetRedirectResponse(threeDSecureUrl, response)
                              : GetSuccessResponse(response));
        }

        public ProcessDetailsResponse Handle3DSecureResponse(FormCollection formCollection)
        {
            var md = formCollection["MD"];
            var paRes = formCollection["PaRes"];

            var response = _secvpn.threeDSecureAuthorisationRequest(new threeDSecureAuthorisationRequestRequest(_paypointSettings.AccountName, _paypointSettings.VPNPassword, _cartModel.CartGuid.ToString(), md, paRes, ""));

            var nameValueCollection = _paypointHelper.ParseEnrolmentResponse(response.threeDSecureAuthorisationRequestReturn);

            var statusCode = nameValueCollection["mpi_status_code"];

            if (string.IsNullOrWhiteSpace(statusCode))
            {
                return new ProcessDetailsResponse
                           {
                               FailureDetails = new FailureDetails
                                                    {
                                                        ErrorCode = nameValueCollection["code"],
                                                        Details = GetErrors(nameValueCollection["code"]),
                                                        Message = nameValueCollection["message"]
                                                    }
                           };
            }
            return statusCode == "229"
                       ? GetFailureResponse(nameValueCollection)
                       : GetSuccessResponse(nameValueCollection);
        }

        private static ProcessDetailsResponse GetRedirectResponse(string threeDSecureUrl, NameValueCollection response)
        {
            return new ProcessDetailsResponse
                       {
                           RedirectDetails = new RedirectDetails
                                                 {
                                                     ACSUrl = HttpUtility.UrlDecode(response["acs_url"]),
                                                     MD = response["MD"],
                                                     PaReq = response["PaReq"],
                                                     TermUrl = threeDSecureUrl
                                                 }
                       };
        }

        private static ProcessDetailsResponse GetFailureResponse(NameValueCollection response)
        {
            CurrentRequestData.ErrorSignal.Raise(new ThreeDSecureException(response));
            return new ProcessDetailsResponse
                       {
                           FailureDetails = new FailureDetails
                                                {
                                                    ErrorCode = response["code"],
                                                    Details = GetErrors(response["code"]),
                                                    Message = response["message"]
                                                }
                       };
        }

        private static IEnumerable<string> GetErrors(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                yield break;

            if (!code.StartsWith("P:", StringComparison.OrdinalIgnoreCase))
                yield return ErrorCodes[code];
            else
            {
                var subcodes = code.Substring(2).ToCharArray();
                foreach (var subcode in subcodes)
                    yield return ErrorCodes["P:" + subcode];
            }
        }

        private static Dictionary<string, string> _errorCodes;
        static Dictionary<string, string> ErrorCodes
        {
            get
            {
                return _errorCodes ??
                       (_errorCodes =
                        new Dictionary<string, string>
                            {
                                {"A", "Transaction authorised by bank. auth_code available as bank reference"},
                                {"N", "Transaction not authorised. Failure message text available to merchant"},
                                {"C", "Communication problem. Trying again later may well work"},
                                {
                                    "F",
                                    "The PayPoint.net system has detected a fraud condition and rejected the transaction. The message field will contain more details."
                                },
                                {"P:A", "Pre-bank checks. Amount not supplied or invalid"},
                                {"P:X", "Pre-bank checks. Not all mandatory parameters supplied"},
                                {"P:P", "Pre-bank checks. Same payment presented twice"},
                                {"P:S", "Pre-bank checks. Start date invalid"},
                                {"P:E", "Pre-bank checks. Expiry date invalid"},
                                {"P:I", "Pre-bank checks. Issue number invalid"},
                                {"P:C", "Pre-bank checks. Card number fails LUHN check (the card number is wrong)"},
                                {"P:T", "Pre-bank checks. Card type invalid - i.e. does not match card number prefix"},
                                {"P:N", "Pre-bank checks. Customer name not supplied"},
                                {"P:M", "Pre-bank checks. Merchant does not exist or not registered yet"},
                                {"P:B", "Pre-bank checks. Merchant account for card type does not exist"},
                                {"P:D", "Pre-bank checks. Merchant account for this currency does not exist"},
                                {"P:V", "Pre-bank checks. CV2 security code mandatory and not supplied / invalid"},
                                {
                                    "P:R",
                                    "Pre-bank checks. Transaction timed out awaiting a virtual circuit. Merchant may not have enough virtual circuits for the volume of business."
                                },
                                {"P:#", "Pre-bank checks. No MD5 hash / token key set up against account"}
                            });

            }
        }
    }
}