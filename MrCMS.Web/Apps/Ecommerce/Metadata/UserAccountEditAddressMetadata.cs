using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class UserAccountEditAddressMetadata : DocumentMetadataMap<UserAccountEditAddress>
    {
        public override string WebGetController
        {
            get { return "UserAccountEditAddress"; }
        }

        public override string IconClass
        {
            get { return "glyphicon glyphicon-user"; }
        }

        public override bool RevealInNavigation
        {
            get { return false; }
        }

        public override bool RequiresParent
        {
            get { return true; }
        }

        public override bool AutoBlacklist
        {
            get { return true; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }

        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }
    }
}