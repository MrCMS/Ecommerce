using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ryness.Entities;

namespace MrCMS.Web.Apps.Ryness.Dbconfiguration
{
    public class TestimonialOverride : IAutoMappingOverride<Testimonial>
    {
        public void Override(AutoMapping<Testimonial> mapping)
        {
            mapping.Map(a => a.Text).CustomType<VarcharMax>().Length(4001);
        }
    }
}