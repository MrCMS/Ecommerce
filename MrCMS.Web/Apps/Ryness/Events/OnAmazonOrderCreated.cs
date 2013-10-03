using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.MessageTemplates;
using MrCMS.Web.Apps.Amazon.Services.Orders.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Ryness.Events
{
    public class OnAmazonOrderCreated : IOnAmazonOrderPlaced
    {
        private readonly IKerridgeService _kerridgeService;

        public OnAmazonOrderCreated(IKerridgeService kerridgeService)
        {
            _kerridgeService = kerridgeService;
        }

        public int Order { get { return 101; } }
        public void IOnAmazonOrderPlaced(AmazonOrder order)
        {
            var orderMrCms = order.Order;

            if (orderMrCms.PaymentStatus.Equals(PaymentStatus.Paid) && !orderMrCms.IsCancelled)
            {
                _kerridgeService.Add(new KerridgeLog
                    {
                        Order = orderMrCms,
                        Sent = false
                    });
            }
        }
    }

    public class SendAmazonOrderShippedEmailToStoreOwner : IOnAmazonOrderPlaced
    {
        private readonly ISession _session;
        private readonly IMessageParser<SendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate, AmazonOrder> _messageParser;

        public SendAmazonOrderShippedEmailToStoreOwner(ISession session,
            IMessageParser<SendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate, AmazonOrder> messageParser)
        {
            _session = session;
            _messageParser = messageParser;
        }

        public int Order { get { return 102; } }
        public void IOnAmazonOrderPlaced(AmazonOrder order)
        {
            var queuedMessage = _messageParser.GetMessage(order);
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }

    }
}