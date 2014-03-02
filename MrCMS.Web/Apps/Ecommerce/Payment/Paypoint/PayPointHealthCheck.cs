using System.Collections.Generic;
using MrCMS.HealthChecks;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Paypoint
{
    public class PayPointHealthCheck : HealthCheck
    {
        private readonly PaypointSettings _settings;

        public PayPointHealthCheck(PaypointSettings settings)
        {
            _settings = settings;
        }

        public override string DisplayName
        {
            get { return "Paypoint Status"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            if (_settings.Enabled && !_settings.IsLive)
                return new HealthCheckResult
                           {
                               Messages = new List<string>
                                              {
                                                  "Paypoint is enabled but is in sandbox mode."
                                              }
                           };

            return HealthCheckResult.Success;
        }
    }
}