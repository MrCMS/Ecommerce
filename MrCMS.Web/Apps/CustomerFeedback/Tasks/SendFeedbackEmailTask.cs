using MrCMS.Tasks;
using MrCMS.Web.Apps.CustomerFeedback.Areas.Admin.Services;

namespace MrCMS.Web.Apps.CustomerFeedback.Tasks
{
    public class SendFeedbackEmailTask : SchedulableTask
    {
        private readonly ISendFeedbackEmails _sendFeedbackEmails;

        public SendFeedbackEmailTask(ISendFeedbackEmails sendFeedbackEmails)
        {
            _sendFeedbackEmails = sendFeedbackEmails;
        }

        public override int Priority
        {
            get { return 0; }
        }

        protected override void OnExecute()
        {
            _sendFeedbackEmails.Send();
        }
    }
}