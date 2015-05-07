using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class UserAcccountAddressesMetadata : DocumentMetadataMap<UserAccountAddresses>
    {
        public override string WebGetController
        {
            get { return "UserAccountAddresses"; }
        }

        public override string IconClass
        {
            get { return "glyphicon glyphicon-user"; }
        }
    }
}