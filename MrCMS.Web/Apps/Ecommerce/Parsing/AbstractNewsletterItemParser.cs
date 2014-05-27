using System;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public abstract class AbstractNewsletterItemParser<T> : INewsletterItemParser<T>
    {
        private Site _site;

        protected Site Site
        {
            get { return _site ?? (_site = MrCMSApplication.Get<Site>()); }
        }

        public abstract string Parse(NewsletterTemplate template, T item);

        protected string ToAbsolute(string urlSegment)
        {
            const string scheme = "http://";

            if (string.IsNullOrWhiteSpace(urlSegment) || urlSegment.StartsWith(scheme, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            string authority = Site.BaseUrl;

            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');

            if (urlSegment.StartsWith("/"))
                urlSegment = urlSegment.TrimStart('/');

            return string.Format("{0}{1}/{2}", scheme, authority, urlSegment);
        }
    }
}