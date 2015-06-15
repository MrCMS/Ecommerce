using MrCMS.Web.Apps.NewsletterBuilder.Entities;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services.Parsing
{
    public interface INewsletterItemParser<in T>
    {
        string Parse(NewsletterTemplate template, T item);
    }
}