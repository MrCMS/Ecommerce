using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.TemplateData;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder
{
    public interface INewsletterProductParser
    {
        string Parse(ProductListTemplateData template, Product item);
    }
}