using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class SessionDataOverride : IAutoMappingOverride<SessionData>
    {
        public void Override(AutoMapping<SessionData> mapping)
        {
            mapping.Map(data => data.Data).MakeVarCharMax();
            mapping.Map(data => data.Key).Index(string.Format("IX_SessionData_Key"));
            mapping.Map(data => data.UserGuid).Index(string.Format("IX_SessionData_UserGuid"));
        }
    }
}