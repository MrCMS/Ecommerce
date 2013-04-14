using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Templating;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class OrderNoteOverride : IAutoMappingOverride<OrderNote>
    {
        public void Override(AutoMapping<OrderNote> mapping)
        {
            mapping.Map(model => model.Note).CustomType<VarcharMax>().Length(4001);
        }
    }
}