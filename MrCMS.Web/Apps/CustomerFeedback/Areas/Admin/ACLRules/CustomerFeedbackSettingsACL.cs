using System.Collections.Generic;
using MrCMS.ACL;

namespace MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.ACLRules
{
    public class CustomerFeedbackSettingsACL : ACLRule
    {
        public const string View = "View";

        public override string DisplayName
        {
            get { return "Customer Feedback Settings"; }
        }

        protected override List<string> GetOperations()
        {
            return new List<string>{ View };
        }
    }
}