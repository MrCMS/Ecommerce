using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class UserRegistrationMetadata : DocumentMetadataMap<UserRegistration>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield return typeof(UserRegistration); }
        }

        public override string App
        {
            get { return "Ecommerce"; }
        }

        public override string WebGetController
        {
            get { return "UserRegistration"; }
        }
        public override string WebGetAction
        {
            get { return "UserRegistration"; }
        }
    }
}