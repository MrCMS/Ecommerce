using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class CurrencyFormattingHelper
    {
        public static string ToCurrencyFormat(this decimal price)
        {
            var currency = MrCMSApplication.Get<EcommerceSettings>().Currency;

            return currency != null ? currency.FormatPrice(price) : price.ToString("0.00");
        }
        public static string ToCurrencyFormat(this decimal? price)
        {
            return price.HasValue ? ToCurrencyFormat(price.Value) : string.Empty;
        }
    }
}