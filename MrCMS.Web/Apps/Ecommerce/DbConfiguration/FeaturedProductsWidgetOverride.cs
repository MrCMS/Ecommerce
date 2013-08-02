using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Widgets;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class FeaturedProductsWidgetOverride : IAutoMappingOverride<FeaturedProducts>
    {
        public void Override(AutoMapping<FeaturedProducts> mapping)
        {
            mapping.Map(x => x.ListOfFeaturedProducts).CustomType<VarcharMax>().Length(4001);
        }
    }
}