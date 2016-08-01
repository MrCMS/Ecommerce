using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Ecommerce.Entities;

namespace MrCMS.Web.Apps.Core.DbConfiguration
{
    public class UserSessionDataOverride : IAutoMappingOverride<SessionData>
    {
        public void Override(AutoMapping<SessionData> mapping)
        {
            mapping.References(x => x.User).Column("UserGuid").PropertyRef(x => x.Guid).ReadOnly();
        }
    }
}