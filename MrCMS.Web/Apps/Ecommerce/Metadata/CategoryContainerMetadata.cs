using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class CategoryContainerMetadata : DocumentMetadataMap<CategoryContainer>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<System.Type> ChildrenList
        {
            get { yield return typeof(Category); }
        }

        public override bool ShowChildrenInAdminNav
        {
            get { return false; }
        }
    }
}