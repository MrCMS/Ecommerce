using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Amazon.Entities.Logs;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Amazon.DbConfiguration
{
    public class LogEntryOverride : IAutoMappingOverride<AmazonLog>
    {
        public void Override(AutoMapping<AmazonLog> mapping)
        {
            mapping.Map(entry => entry.Message).CustomType<VarcharMax>().Length(4001);
            mapping.Map(entry => entry.Detail).CustomType<VarcharMax>().Length(4001);

            //mapping.ReferencesAny(item => item.AmazonOrder).AutoMap("ItemType", "ItemId");
            //mapping.ReferencesAny(item => item.AmazonListing).AutoMap("ItemType", "ItemId").;
        }
    }
}