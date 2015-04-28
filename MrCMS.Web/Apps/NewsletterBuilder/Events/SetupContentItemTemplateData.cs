using System.Linq;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Services;

namespace MrCMS.Web.Apps.NewsletterBuilder.Events
{
    public class SetupContentItemTemplateData : IOnAdded<NewsletterTemplate>
    {
        private readonly IGetTemplateDataForNewTemplate _getTemplateDataForNewTemplate;

        public SetupContentItemTemplateData(IGetTemplateDataForNewTemplate getTemplateDataForNewTemplate)
        {
            _getTemplateDataForNewTemplate = getTemplateDataForNewTemplate;
        }

        public void Execute(OnAddedArgs<NewsletterTemplate> args)
        {
            var contentItemTemplateDatas = _getTemplateDataForNewTemplate.Get();
            if (!contentItemTemplateDatas.Any())
                return;
            args.Session.Transact(session => contentItemTemplateDatas.ForEach(data =>
            {
                data.NewsletterTemplate = args.Item;
                session.Save(data);
            }));
        }
    }
}