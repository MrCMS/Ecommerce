using MrCMS.Apps;
using MrCMS.Web.Apps.CustomerFeedback.Controllers;
using Ninject;

namespace MrCMS.Web.Apps.CustomerFeedback
{
    public class CustomerFeedbackApp : MrCMSApp
    {
        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            // Order Feedback
            context.MapRoute("Order Feedback Post", "Apps/CustomerFeedback/OrderFeedback/Handle",
                new {controller = "OrderFeedback", action = "Submit"},
                new[] {typeof (OrderFeedbackController).Namespace});
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