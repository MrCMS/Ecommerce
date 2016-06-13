using System.Collections.Generic;

namespace MrCMS.ACL.Rules
{
    public class HealthChecksACL : ACLRule
    {
        public const string Show = "Show";

        public override string DisplayName => "Health Checks";

        protected override List<string> GetOperations()
        {
            return new List<string>
            {
                Show
            };
        }
    }
}