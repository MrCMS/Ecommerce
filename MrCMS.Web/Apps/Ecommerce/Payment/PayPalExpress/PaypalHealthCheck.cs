using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.HealthChecks;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PaypalHealthCheck : HealthCheck
    {
        private readonly PayPalExpressCheckoutSettings _settings;

        public PaypalHealthCheck(PayPalExpressCheckoutSettings settings)
        {
            _settings = settings;
        }

        public override string DisplayName
        {
            get { return "Paypal Status"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            if (_settings.Enabled && !_settings.IsLive)
                return new HealthCheckResult
                           {
                               Messages = new List<string>
                                              {
                                                  "Paypal Express is enabled but is in sandbox mode."
                                              }
                           };

            return HealthCheckResult.Success;
        }
    }
}