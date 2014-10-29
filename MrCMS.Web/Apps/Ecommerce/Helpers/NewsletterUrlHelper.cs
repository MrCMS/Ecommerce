using System;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public class NewsletterUrlHelper : INewsletterUrlHelper
    {
        private const string scheme = "http";
        private readonly Site _site;

        public NewsletterUrlHelper(Site site)
        {
            _site = site;
        }

        public string ToAbsolute(string urlSegment)
        {
            if (string.IsNullOrWhiteSpace(urlSegment))
            {
                return string.Empty;
            }

            if (urlSegment.StartsWith(scheme, StringComparison.OrdinalIgnoreCase))
            {
                return urlSegment;
            }

            string authority = _site.BaseUrl;

            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');

            if (urlSegment.StartsWith("/"))
                urlSegment = urlSegment.TrimStart('/');

            return string.Format("{0}://{1}/{2}", scheme, authority, urlSegment);
        }
    }
}