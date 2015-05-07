using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class UserAcccountReviewsMetadata : DocumentMetadataMap<UserAccountReviews>
    {
        public override string WebGetController
        {
            get { return "UserAccountReviews"; }
        }

        public override string IconClass
        {
            get { return "glyphicon glyphicon-user"; }
        }
    }
}