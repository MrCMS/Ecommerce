using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class CartItemOverride : IAutoMappingOverride<CartItem>
    {
        public void Override(AutoMapping<CartItem> mapping)
        {
            mapping.Map(item => item.Data).MakeVarCharMax();
        }
    }
}