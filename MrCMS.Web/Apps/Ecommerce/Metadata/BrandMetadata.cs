using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class BrandMetadata : DocumentMetadataMap<Brand>
    {
        public override string WebGetController
        {
            get { return "Brand"; }
        }

        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }

        public override bool RequiresParent
        {
            get { return true; }
        }
    }
}