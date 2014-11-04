using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class GiftCardUsageOverride : IAutoMappingOverride<GiftCardUsage>
    {
        public void Override(AutoMapping<GiftCardUsage> mapping)
        {
            mapping.Map(card => card.Amount).Scale(2);
        }
    }
}