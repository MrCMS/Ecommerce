using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class GoogleBaseACL : ACLRule
    {
        public const string View = "View";
        public const string CanExport = "Can Export";
        public const string Save = "Save Settings";
        public const string GoogleBaseProducts = "Update Google Base Products";

        public override string DisplayName
        {
            get { return "Google Base"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View, CanExport, Save, GoogleBaseProducts };
        }
    }
}