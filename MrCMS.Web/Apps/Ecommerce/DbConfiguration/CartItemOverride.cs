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

    public class ProductOverride: IAutoMappingOverride<Product>
    {
        public void Override(AutoMapping<Product> mapping)
        {
            mapping.HasManyToMany(product => product.Categories).Table("ProductCategories").Not.Inverse();
        }
    }

    public class CategoryOverride: IAutoMappingOverride<Category>
    {
        public void Override(AutoMapping<Category> mapping)
        {
            mapping.HasManyToMany(category => category.Products).Table("ProductCategories").Inverse();
        }
    }
}