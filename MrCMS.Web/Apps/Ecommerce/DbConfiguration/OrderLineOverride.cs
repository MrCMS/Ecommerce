using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class OrderLineOverride : IAutoMappingOverride<OrderLine>
    {
        public void Override(AutoMapping<OrderLine> mapping)
        {
            mapping.ReferencesAny(item => item.ProductVariant).AutoMap("ItemType", "ItemId");
        }
    }
}