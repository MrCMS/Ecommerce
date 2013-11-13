using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.SagePay;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public class TransactionRegistrar : ITransactionRegistrar
    {
        private readonly SagePaySettings _sagePaySettings;
        private readonly IHttpRequestSender _httpRequestSender;
        private readonly ITransactionRegistrationBuilder _transactionRegistrationBuilder;

        public TransactionRegistrar(SagePaySettings sagePaySettings,
                                    IHttpRequestSender httpRequestSender, 
                                    ITransactionRegistrationBuilder transactionRegistrationBuilder)
        {
            _sagePaySettings = sagePaySettings;
            _httpRequestSender = httpRequestSender;
            _transactionRegistrationBuilder = transactionRegistrationBuilder;
        }

        public TransactionRegistrationResponse Send(CartModel cartModel)
        {
            string sagePayUrl = _sagePaySettings.RegistrationUrl;

            var registration = _transactionRegistrationBuilder.Build(cartModel);

            var serializer = new HttpPostSerializer();
            var postData = serializer.Serialize(registration);

            var response = _httpRequestSender.SendRequest(sagePayUrl, postData);

            var deserializer = new ResponseSerializer();
            return deserializer.Deserialize<TransactionRegistrationResponse>(response);
        }
    }
}