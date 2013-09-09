using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class NewInProductsMetaData : DocumentMetadataMap<NewInProducts>
    {
        public override string App
        {
            get { return "Ecommerce"; }
        }
        
        public override string WebGetController
        {
            get { return "NewInProducts"; }
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