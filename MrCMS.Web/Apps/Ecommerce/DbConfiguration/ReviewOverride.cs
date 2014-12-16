using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ReviewOverride : IAutoMappingOverride<ProductReview>
    {
        public void Override(AutoMapping<ProductReview> mapping)
        {
            mapping.Map(x => x.Text).CustomType<VarcharMax>().Length(4001);
        }
    }
}