using System;
using System.Web;
using MrCMS.Website.Profiling;

namespace MrCMS.Web.Apps.Ecommerce.Services.Brands
{
    public class EnableMiniProfilerForBrandSearch : IReasonToEnableMiniProfiler
    {
        public bool ShouldEnableFor(HttpRequest request)
        {
            if (request.Url.AbsolutePath.Equals("/brand/search/query", StringComparison.OrdinalIgnoreCase))
                return true;
            if (request.Url.AbsolutePath.Equals("/brand/search/results", StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
}