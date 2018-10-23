using System;

namespace MrCMS.AmazonApp.Tests.Helpers
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