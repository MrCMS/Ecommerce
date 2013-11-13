using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Payment.Paypoint;
using MrCMS.Web.Apps.Ecommerce.Services.Paypoint;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class PaypointResponseOverride : IAutoMappingOverride<PaypointResponse>
    {
        public void Override(AutoMapping<PaypointResponse> mapping)
        {
            mapping.Map(response => response.RawData).MakeVarCharMax();
            mapping.Map(response => response.Response).MakeVarCharMax();
        }
    }
}