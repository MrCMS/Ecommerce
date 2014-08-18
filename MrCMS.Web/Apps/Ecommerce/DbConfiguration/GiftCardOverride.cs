using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class GiftCardOverride : IAutoMappingOverride<GiftCard>
    {
        public void Override(AutoMapping<GiftCard> mapping)
        {
            mapping.Map(card => card.Value).Scale(2);

            mapping.Map(card => card.RecipientName).Length(100);
            mapping.Map(card => card.RecipientEmail).Length(100);
            mapping.Map(card => card.SenderName).Length(100);
            mapping.Map(card => card.SenderEmail).Length(100);
            mapping.Map(card => card.Message).Length(1000);
        }
    }
}