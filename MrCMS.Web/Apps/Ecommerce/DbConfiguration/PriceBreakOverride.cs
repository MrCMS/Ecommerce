using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class PriceBreakOverride : IAutoMappingOverride<PriceBreak>
    {
        public void Override(AutoMapping<PriceBreak> mapping)
        {
            mapping.Map(x => x.Quantity).Index("IX_PriceBreak_Quanitity");
        }
    }
}