using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class ProductReviewSettingsAcl : ACLRule
    {
        public const string Edit = "Edit";

        public override string DisplayName
        {
            get { return "Product Review Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> {Edit};
        }
    }
}