using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class UserAcccountRewardPointsMetadata : DocumentMetadataMap<UserAccountRewardPoints>
    {
        public override string WebGetController
        {
            get { return "UserAccountRewardPoints"; }
        }

        public override string IconClass
        {
            get { return "glyphicon glyphicon-user"; }
        }
    }
}