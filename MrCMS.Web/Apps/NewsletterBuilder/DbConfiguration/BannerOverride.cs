using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.DbConfiguration
{
    public class BannerOverride : IAutoMappingOverride<Banner>
    {
        public void Override(AutoMapping<Banner> mapping)
        {
            mapping.Map(x => x.ImageUrl).Length(1000);
            mapping.Map(x => x.LinkUrl).Length(1000);
        }
    }
}