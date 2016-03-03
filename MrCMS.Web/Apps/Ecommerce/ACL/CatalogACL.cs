using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class CatalogACL : ACLRule
    {
        public const string Show = "Show";

        public override string DisplayName => "Catalog";

        protected override List<string> GetOperations()
        {
            return new List<string> { Show };
        }
    }
}