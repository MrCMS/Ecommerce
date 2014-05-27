using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public interface INewsletterItemParser<in T>
    {
        string Parse(NewsletterTemplate template, T item);
    }
}