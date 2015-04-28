using System.Collections.Generic;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services
{
    public interface IGetTemplateDataForNewTemplate
    {
        IList<ContentItemTemplateData> Get();
    }
}