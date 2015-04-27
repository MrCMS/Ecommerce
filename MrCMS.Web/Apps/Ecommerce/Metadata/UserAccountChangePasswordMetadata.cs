using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class UserAccountChangePasswordMetadata : DocumentMetadataMap<UserAccountChangePassword>
    {
        public override string WebGetController
        {
            get { return "UserAccountChangePassword"; }
        }

        public override string IconClass
        {
            get { return "glyphicon glyphicon-user"; }
        }
    }
}