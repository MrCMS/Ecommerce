using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class EcommerceSettingsExtensions
    {
        public static Currency Currency(this EcommerceSettings settings)
        {
            var session = MrCMSApplication.Get<ISession>();
            return settings.CurrencyId > 0
                ? session.Get<Currency>(settings.CurrencyId)
                : session.QueryOver<Currency>().Take(1).SingleOrDefault();
        }
        
        public static string CurrencyCode(this EcommerceSettings settings)
        {
            var currency = settings.Currency();
            return currency == null ? string.Empty : currency.Code;
        }
    }
}