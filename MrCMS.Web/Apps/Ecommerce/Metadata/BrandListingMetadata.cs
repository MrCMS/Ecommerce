using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class BrandListingMetadata : DocumentMetadataMap<BrandListing>
    {
        public override string WebGetController
        {
            get { return "BrandListing"; }
        }

        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield return typeof(Brand); }
        }

        public override string IconClass
        {
            get { return "glyphicon glyphicon-list"; }
        }
    }
}