using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class AdminMenuACL : ACLRule
    {
        public const string Pages = "Pages";
        public const string Media = "Media";
        public const string Layouts = "Layouts";

        public override string DisplayName => "Admin Menu";

        protected override List<string> GetOperations()
        {
            return new List<string>
            {
                Pages,
                Media,
                Layouts
            };
        }
    }
}