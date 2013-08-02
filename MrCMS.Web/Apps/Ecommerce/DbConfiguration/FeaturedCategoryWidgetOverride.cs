using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Widgets;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class FeaturedCategoryWidgetOverride : IAutoMappingOverride<FeaturedCategories>
    {
        public void Override(AutoMapping<FeaturedCategories> mapping)
        {
            mapping.Map(x => x.ListOfFeaturedCategories).CustomType<VarcharMax>().Length(4001);
        }
    }
}