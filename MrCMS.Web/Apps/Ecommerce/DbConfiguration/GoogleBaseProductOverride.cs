using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class GoogleBaseProductOverride : IAutoMappingOverride<GoogleBaseProduct>
    {
        public void Override(AutoMapping<GoogleBaseProduct> mapping)
        {
            mapping.References(product => product.ProductVariant).Not.Nullable();
        }
    }
}