using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class UserAccountInfoMetadata : DocumentMetadataMap<UserAccountInfo>
    {
        public override string WebGetController
        {
            get { return "UserAccountInfo"; }
        }

        public override string IconClass
        {
            get { return "glyphicon glyphicon-user"; }
        }
    }
}