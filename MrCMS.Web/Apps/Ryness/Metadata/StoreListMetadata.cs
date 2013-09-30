using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ryness.Pages;

namespace MrCMS.Web.Apps.Ryness.Metadata
{
    public class StoreListMetadata : DocumentMetadataMap<StoreList>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override System.Collections.Generic.IEnumerable<System.Type> ChildrenList
        {
            get { yield return typeof(Store); }
        }

        public override string IconClass
        {
            get { return "icon-list"; }
        }

        public override int MaxChildNodes
        {
            get { return 5; }
        }

        public override string DefaultLayoutName
        {
            get { return "Content Page Layout"; }
        }
    }
}