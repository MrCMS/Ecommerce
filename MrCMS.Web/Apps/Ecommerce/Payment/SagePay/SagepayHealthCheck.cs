using System.Collections.Generic;
using MrCMS.HealthChecks;

namespace MrCMS.Web.Apps.Ecommerce.Payment.SagePay
{
    public class SagepayHealthCheck : HealthCheck
    {
        private readonly SagePaySettings _settings;

        public SagepayHealthCheck(SagePaySettings settings)
        {
            _settings = settings;
        }

        public override string DisplayName
        {
            get { return "Sagepay Status"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            if (_settings.Enabled && !_settings.Mode.Equals(VspServerMode.Live))
                return new HealthCheckResult
                           {
                               Messages = new List<string>
                                              {
                                                  "Sagepay is enabled but is in sandbox mode."
                                              }
                           };

            return HealthCheckResult.Success;
        }
    }
}