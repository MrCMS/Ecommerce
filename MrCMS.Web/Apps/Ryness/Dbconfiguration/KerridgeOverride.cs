using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ryness.Entities;

namespace MrCMS.Web.Apps.Ryness.Dbconfiguration
{
    public class KerridgeOverride : IAutoMappingOverride<KerridgeLog>
    {
        public void Override(AutoMapping<KerridgeLog> mapping)
        {
        }
    }
}