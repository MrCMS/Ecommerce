using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class ImageRightAndTextLeftOverride : IAutoMappingOverride<ImageRightAndTextLeft>
    {
        public void Override(AutoMapping<ImageRightAndTextLeft> mapping)
        {
            mapping.Map(x => x.Text).CustomType<VarcharMax>().Length(4001);
        }
    }

    public class ImageLeftAndTextRightOverride : IAutoMappingOverride<ImageLeftAndTextRight>
    {
        public void Override(AutoMapping<ImageLeftAndTextRight> mapping)
        {
            mapping.Map(x => x.Text).CustomType<VarcharMax>().Length(4001);
        }
    }
}