using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class EcommerceSearchablePageOverride : IAutoMappingOverride<EcommerceSearchablePage>
    {
        public void Override(AutoMapping<EcommerceSearchablePage> mapping)
        {
            mapping.HasManyToMany(category => category.HiddenSearchSpecifications)
                .Table("Ecommerce_SearchablePageHiddenSearchSpecifications");
        }
    }
}