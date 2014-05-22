using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;

namespace MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder
{
    public interface INewsletterTemplateService
    {
        IList<NewsletterTemplate> GetAll();
        IPagedList<NewsletterTemplate> GetAllPaged();
        void Add(NewsletterTemplate newsletterTemplate);
        void Edit(NewsletterTemplate newsletterTemplate);
        void Delete(NewsletterTemplate newsletterTemplate);
    }
}