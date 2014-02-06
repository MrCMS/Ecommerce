using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public class TransactionManager : ITransactionManager
    {
        private readonly SagePaySettings _sagePaySettings;
        private readonly IHttpRequestSender _httpRequestSender;
        private readonly ITransactionRegistrationBuilder _transactionRegistrationBuilder;

        public TransactionManager(SagePaySettings sagePaySettings,
                                    IHttpRequestSender httpRequestSender,
                                    ITransactionRegistrationBuilder transactionRegistrationBuilder)
        {
            _sagePaySettings = sagePaySettings;
            _httpRequestSender = httpRequestSender;
            _transactionRegistrationBuilder = transactionRegistrationBuilder;
        }

        public TransactionRegistrationResponse Register(CartModel cartModel)
        {
            string sagePayUrl = _sagePaySettings.RegistrationUrl;

            var registration = _transactionRegistrationBuilder.BuildRegistration(cartModel);

            var serializer = new HttpPostSerializer();
            var postData = serializer.Serialize(registration);

            var response = _httpRequestSender.SendRequest(sagePayUrl, postData);

            var deserializer = new ResponseSerializer();
            var registrationResponse = deserializer.Deserialize<TransactionRegistrationResponse>(response);
            registrationResponse.VendorTxCode = registration.VendorTxCode;
            registrationResponse.CartTotal = cartModel.Total;
            return registrationResponse;
        }
    }

    public class VoidResponse
    {
        /// <summary>
        /// Protocol version
        /// </summary>
        public string VPSProtocol { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public ResponseType Status { get; set; }

        /// <summary>
        /// Additional status details
        /// </summary>
        public string StatusDetail { get; set; }
    }
}