using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ProductOverride : IAutoMappingOverride<Product>
    {
        public void Override(AutoMapping<Product> mapping)
        {
            mapping.Map(product => product.Abstract).Length(500);
            mapping.HasManyToMany(product => product.Categories)
                   .Table("Ecommerce_ProductCategories")
                   .AsList(part => part.Column("DisplayOrder"))
                   .Not.Inverse();
            mapping.HasManyToMany(product => product.Options).Table("Ecommerce_ProductOptions")
                   .AsList(part => part.Column("DisplayOrder"))
                   .Not.Inverse();
        }
    }
}