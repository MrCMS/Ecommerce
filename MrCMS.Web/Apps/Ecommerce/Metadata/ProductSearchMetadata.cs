using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class ProductSearchMetadata : DocumentMetadataMap<ProductSearch>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield return typeof(Product); }
        }

        public override bool ShowChildrenInAdminNav { get { return false; } }

        public override string WebGetController
        {
            get { return "ProductSearch"; }
        }
        public override string WebGetAction
        {
            get { return "Show"; }
        }
        public override bool ChildrenMaintainHierarchy
        {
            get { return false; }
        }
        public override string DefaultLayoutName
        {
            get
            {
                return "Search Layout";
            }
        }
    }
}