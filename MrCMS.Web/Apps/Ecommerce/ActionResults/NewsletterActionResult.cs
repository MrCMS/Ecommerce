using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.Ecommerce.Parsing;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.ActionResults
{
    public class NewsletterActionResult : ActionResult
    {
        private static readonly Regex BodyRegex = new Regex(@"\[(?i)BodyContent\]");

        private static readonly MethodInfo ParseMethod = typeof (NewsletterActionResult)
            .GetMethod("ParseItem", BindingFlags.NonPublic | BindingFlags.Static);

        private readonly Newsletter _newsletter;
        private readonly NewsletterTemplate _template;

        public NewsletterActionResult(Newsletter newsletter, NewsletterTemplate templateOverride = null)
        {
            _newsletter = newsletter;
            _template = templateOverride ?? newsletter.NewsletterTemplate;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write(ParseTemplate());
        }

        private string ParseTemplate()
        {
            string bodyContent = string.Empty;

            if (_newsletter.ContentItems.Any())
            {
                bodyContent += ParseItem(_newsletter.ContentItems.First());

                foreach (ContentItem item in _newsletter.ContentItems.Skip(1))
                {
                    bodyContent += _template.Divider;
                    bodyContent += ParseItem(item);
                }
            }

            return BodyRegex.Replace(_template.BaseTemplate, bodyContent);
        }

        private string ParseItem(ContentItem item)
        {
            return (string) ParseMethod.MakeGenericMethod(item.GetType()).Invoke(this, new object[] {_template, item});
        }

        // ReSharper disable UnusedMember.Local - Called via reflection
        private static string ParseItem<T>(NewsletterTemplate template, T item)
        {
            var parser = MrCMSApplication.Get<INewsletterItemParser<T>>();
            return parser.Parse(template, item);
        }

        // ReSharper restore UnusedMember.Local
    }
}