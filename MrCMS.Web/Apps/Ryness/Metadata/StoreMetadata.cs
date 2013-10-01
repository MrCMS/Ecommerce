using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ryness.Pages;

namespace MrCMS.Web.Apps.Ryness.Metadata
{
    public class StoreMetadata : DocumentMetadataMap<Store>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override System.Collections.Generic.IEnumerable<System.Type> ChildrenList
        {
            get { yield break; }
        }

        public override bool RequiresParent
        {
            get { return true; }
        }

        public override bool AutoBlacklist
        {
            get { return true; }
        }

        public override string DefaultLayoutName
        {
            get { return "Content Page Layout"; }
        }

        public override string IconClass
        {
            get { return "icon-shopping-cart"; }
        }
    }
}