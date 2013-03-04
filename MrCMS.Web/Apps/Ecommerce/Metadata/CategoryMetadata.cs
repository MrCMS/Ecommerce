using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class CategoryMetadata : DocumentMetadataMap<Category>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<System.Type> ChildrenList
        {
            get { yield return typeof(Category); }
        }

        public override bool RequiresParent { get { return true; } }
        public override bool AutoBlacklist { get { return true; } }
    }
}