using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services
{
    public class GetTemplateDataForNewTemplate : IGetTemplateDataForNewTemplate
    {
        public IList<ContentItemTemplateData> Get()
        {
            var types = TypeHelper.GetAllConcreteMappedClassesAssignableFrom<ContentItemTemplateData>();
            return types.Select(Activator.CreateInstance).OfType<ContentItemTemplateData>().ToList();
        }
    }
}