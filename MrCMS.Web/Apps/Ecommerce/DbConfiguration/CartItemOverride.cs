using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class CartItemOverride : IAutoMappingOverride<CartItem>
    {
        public void Override(AutoMapping<CartItem> mapping)
        {
            mapping.ReferencesAny(item => item.Item).AutoMap("ItemType", "ItemId");
        }
    }
}