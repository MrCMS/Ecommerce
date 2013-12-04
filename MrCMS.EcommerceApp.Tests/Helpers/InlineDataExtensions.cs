using System;

namespace MrCMS.EcommerceApp.Tests.Helpers
{
    public static class InlineDataExtensions
    {
        public static decimal ToDecimal(this double d)
        {
            return Convert.ToDecimal(d);
        }
        public static decimal? ToDecimal(this double? d)
        {
            return d.HasValue ? (decimal?) Convert.ToDecimal(d) : null;
        }
    }
}