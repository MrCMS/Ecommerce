using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class OrderLineOverride : IAutoMappingOverride<OrderLine>
    {
        public void Override(AutoMapping<OrderLine> mapping)
        {
            mapping.References(item => item.ProductVariant).Column("ItemId");
        }
    }
}