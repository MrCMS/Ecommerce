using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.DbConfiguration
{
    public class ImageRightAndTextLeftOverride : IAutoMappingOverride<ImageRightAndTextLeft>
    {
        public void Override(AutoMapping<ImageRightAndTextLeft> mapping)
        {
            mapping.Map(x => x.Text).MakeVarCharMax();
            mapping.Map(x => x.ImageUrl).Length(1000);
        }
    }
}