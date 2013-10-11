using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.DbConfiguration
{
    public class AmazonOrderSyncDataOverride : IAutoMappingOverride<AmazonOrderSyncData>
    {
        public void Override(AutoMapping<AmazonOrderSyncData> mapping)
        {
            mapping.Map(model => model.Data).CustomType<VarcharMax>().Length(4001);
        }
    }
}