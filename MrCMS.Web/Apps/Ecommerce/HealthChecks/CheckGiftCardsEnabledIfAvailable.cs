using System.Collections.Generic;
using MrCMS.HealthChecks;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.HealthChecks
{
    public class CheckGiftCardsEnabledIfAvailable : HealthCheck
    {
        private readonly ISession _session;
        private readonly EcommerceSettings _ecommerceSettings;

        public CheckGiftCardsEnabledIfAvailable(ISession session, EcommerceSettings ecommerceSettings)
        {
            _session = session;
            _ecommerceSettings = ecommerceSettings;
        }

        public override string DisplayName
        {
            get { return "Gift Card Check"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var giftCards = _session.QueryOver<ProductVariant>().Where(x => x.IsGiftCard).Cacheable().RowCount();
            if (giftCards > 0 && !_ecommerceSettings.GiftCardsEnabled)
            {
                return new HealthCheckResult
                {
                    Messages = new List<string>
                    {
                        string.Format("Gift cards are not enabled but there are {0} gift card{1} added in the system. Enable giftcards from settings so people can use gift cards.", giftCards, (giftCards == 1 ? "" : "s")),
                    },
                    OK = false
                };
            }
            return new HealthCheckResult
            {
                OK = true
            };
        }
    }
}