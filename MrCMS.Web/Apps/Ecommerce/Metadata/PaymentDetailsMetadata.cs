using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata.CartAndCheckout
{
    public class PaymentDetailsMetadata : DocumentMetadataMap<PaymentDetails>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get { yield return typeof(OrderPlaced); }
        }

        public override bool RequiresParent { get { return true; } }
        public override bool AutoBlacklist { get { return true; } }

        public override string DefaultLayoutName
        {
            get { return "Checkout Layout"; }
        }

        public override string App
        {
            get { return "Ecommerce"; }
        }

        public override string WebGetController
        {
            get { return "PaymentDetails"; }
        }
        public override string WebGetAction
        {
            get { return "Show"; }
        }
        public override bool ChildrenMaintainHierarchy
        {
            get { return false; }
        }
    }
}