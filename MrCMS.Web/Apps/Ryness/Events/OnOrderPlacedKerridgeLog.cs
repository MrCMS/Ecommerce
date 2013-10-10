using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Ryness.Events
{
    public class OnOrderPlacedKerridgeLog : IOnOrderPlaced
    {
        private readonly IKerridgeService _kerridgeService;
        private readonly ISession _session;

        public OnOrderPlacedKerridgeLog(IKerridgeService kerridgeService, ISession session)
        {
            _kerridgeService = kerridgeService;
            _session = session;
        }

        public int Order { get { return 100; } }
        public void OnOrderPlaced(Order order)
        {
            if (order.PaymentStatus.Equals(PaymentStatus.Paid))
            {
                var kerridgeLog = new KerridgeLog
                    {
                        Order = order,
                        Sent = false
                    };
                _kerridgeService.Add(kerridgeLog);
            }
        }
    }
}