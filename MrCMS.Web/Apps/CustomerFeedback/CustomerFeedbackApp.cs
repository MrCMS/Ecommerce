using MrCMS.Apps;
using Ninject;

namespace MrCMS.Web.Apps.CustomerFeedback
{
    public class CustomerFeedbackApp : MrCMSApp
    {
        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }

        public override string AppName
        {
            get { return "CustomerFeedback"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
        }
    }
}