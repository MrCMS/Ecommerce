using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Templating;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class NotificationTemplateSettingsOverride : IAutoMappingOverride<MessageTemplateSettings>
    {
        public void Override(AutoMapping<MessageTemplateSettings> mapping)
        {
            mapping.Map(model => model.OrderConfirmationTemplate).CustomType<VarcharMax>().Length(4001);
            mapping.Map(model => model.CancelledMessageTemplate).CustomType<VarcharMax>().Length(4001);
            mapping.Map(model => model.ShippingMessageTemplate).CustomType<VarcharMax>().Length(4001);
            mapping.Map(model => model.OwnerTemplate).CustomType<VarcharMax>().Length(4001);
        }
    }
}