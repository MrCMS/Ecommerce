using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class ProductMetadata : DocumentMetadataMap<Product>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }

        public override bool RequiresParent { get { return true; } }
        public override bool AutoBlacklist { get { return true; } }

        public override string EditPartialView
        {
            get { return "ProductAdmin"; }
        }

        public override string App
        {
            get { return "Ecommerce"; }
        }
    }
}