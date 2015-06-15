using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services
{
    public interface INewsletterTemplateAdminService
    {
        IList<NewsletterTemplate> GetAll();
        void Add(NewsletterTemplate newsletterTemplate);
        void Edit(NewsletterTemplate newsletterTemplate);
        void Delete(NewsletterTemplate newsletterTemplate);
        HashSet<Type> GetTemplateDataOptions();
        
    }
}