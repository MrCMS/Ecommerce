using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.UrlGenerators
{
    public class CategoryWithHierarchyUrlGenerator : WebpageUrlGenerator<Category>
    {
        private readonly EcommerceSettings _settings;

        public CategoryWithHierarchyUrlGenerator(EcommerceSettings settings)
        {
            _settings = settings;
        }

        public override string GetUrl(string pageName, Webpage parent, bool useHierarchy)
        {
            var prefix = !string.IsNullOrWhiteSpace(_settings.CategoryUrl) ? _settings.CategoryUrl : "{0}";

            var stringBuilder = new StringBuilder();

            if (parent != null)
            {
                var tidyUrl = GetFormattedUrl(parent);
                if(!string.IsNullOrWhiteSpace(tidyUrl))
                    stringBuilder.Append(tidyUrl + "/");
            }

            stringBuilder.Append(SeoHelper.TidyUrl(pageName));

            var url = stringBuilder.ToString();
            
            return string.Format(prefix, url);
        }

        private static string GetFormattedUrl(Webpage parent)
        {
            var parts = new List<string>();
            var temp = parent as Category;
            while (temp != null)
            {
                var urlSegment = temp.UrlSegment;
                var urlParts = urlSegment.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                if (!urlParts.Any())
                {
                    break;
                }
                parts.Insert(0, urlParts.Last());
                temp = temp.Parent as Category;
            }

            return string.Join("/", parts);
        }
    }
}