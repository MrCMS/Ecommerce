using FakeItEasy;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;

namespace MrCMS.EcommerceApp.Tests.Payment.PayPalExpress
{
    public class PayPalRequestServiceTests
    {
        private readonly IPayPalUrlService _payPalUrlService;
        private readonly IPayPalShippingService _payPalShippingService;
        private readonly IPayPalOrderService _payPalOrderService;
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;
        private readonly PayPalRequestService _payPalRequestService;

        public PayPalRequestServiceTests()
        {
            _payPalUrlService = A.Fake<IPayPalUrlService>();
            _payPalShippingService = A.Fake<IPayPalShippingService>();
            _payPalOrderService = A.Fake<IPayPalOrderService>();
            _payPalExpressCheckoutSettings = new PayPalExpressCheckoutSettings();
            _payPalRequestService = new PayPalRequestService(_payPalUrlService, _payPalShippingService, _payPalOrderService, _payPalExpressCheckoutSettings);
        }
    }
}