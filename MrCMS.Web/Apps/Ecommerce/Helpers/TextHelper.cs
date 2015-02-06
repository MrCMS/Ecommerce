using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class TextHelper
    {
        public static MvcHtmlString PlainTextToParagraphs(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return MvcHtmlString.Empty;
            var paragraphs = text.Split(new[]{'\n','\r'},StringSplitOptions.RemoveEmptyEntries);
            return MvcHtmlString.Create(string.Join("", paragraphs.Select(s => "<p>" + s.StripHtml() + "</p>")));
        }
    }
}