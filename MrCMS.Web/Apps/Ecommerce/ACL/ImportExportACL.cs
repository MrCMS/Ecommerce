using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.Ecommerce.ACL
{
    public class ImportExportACL : ACLRule
    {
        public const string View = "View";
        public const string CanImport = "Can Import";
        public const string CanExport = "Can Export";

        public override string DisplayName
        {
            get { return "Import Export Report"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string> { View, CanImport, CanExport };
        }
    }
}