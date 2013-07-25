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

        public override string App
        {
            get { return "Ecommerce"; }
        }

        public override string WebGetController
        {
            get { return "Product"; }
        }
        public override string WebGetAction
        {
            get { return "Show"; }
        }

        public override string DefaultLayoutName
        {
            get
            {
                return "Product Layout";
            }
        }
    }
}