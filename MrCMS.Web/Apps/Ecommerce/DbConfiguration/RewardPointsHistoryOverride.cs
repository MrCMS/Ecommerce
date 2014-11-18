using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.RewardPoints;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class RewardPointsHistoryOverride : IAutoMappingOverride<RewardPointsHistory>
    {
        public void Override(AutoMapping<RewardPointsHistory> mapping)
        {
            mapping.Map(history => history.Message).MakeVarCharMax();
        }
    }
}