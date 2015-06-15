using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class UserAcccountOrdersMetadata : DocumentMetadataMap<UserAccountOrders>
    {
        public override string WebGetController
        {
            get { return "UserAccountOrders"; }
        }

        public override string IconClass
        {
            get { return "glyphicon glyphicon-user"; }
        }
    }
}