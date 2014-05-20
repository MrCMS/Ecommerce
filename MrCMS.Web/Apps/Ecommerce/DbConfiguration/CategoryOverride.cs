using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class CategoryOverride : IAutoMappingOverride<Category>
    {
        public void Override(AutoMapping<Category> mapping)
        {
            mapping.Map(category => category.Abstract).Length(500);
            mapping.HasManyToMany(category => category.Products).Table("Ecommerce_ProductCategories").Inverse();
            mapping.HasManyToMany(category => category.HiddenSearchSpecifications).Table("Ecommerce_CategoryHiddenSearchSpecifications");
        }
    }
}