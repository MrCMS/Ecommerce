﻿using System;
using System.Collections.Generic;
using MrCMS.Apps;
using MrCMS.Web.Apps.CustomerFeedback.Controllers;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
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


            context.MapRoute("Customer Interaction Post", "Apps/CustomerFeedback/CustomerInteraction/Handle",
                new {controller = "CustomerInteraction", action = "Submit"},
                new[] {typeof (CustomerInteractionController).Namespace});
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

        public override IEnumerable<Type> BaseTypes
        {
            get { yield return typeof (Feedback); }
        }
    }
}