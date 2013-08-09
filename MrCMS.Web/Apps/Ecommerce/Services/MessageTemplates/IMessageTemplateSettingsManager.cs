using MrCMS.Web.Apps.Ecommerce.Entities.Templating;

namespace MrCMS.Web.Apps.Ecommerce.Services.MessageTemplates
{
    public interface IMessageTemplateSettingsManager
    {
        MessageTemplateSettings Get();
        void Save(MessageTemplateSettings item);
        void Delete(MessageTemplateSettings item);
    }
}