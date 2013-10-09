using System;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Orders.Events;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Services;

namespace MrCMS.Web.Apps.Ryness.Events
{
    public class AmazonCreateKerridgeLog : IOnAmazonOrderPlaced
    {
        private readonly IKerridgeService _kerridgeService;

        public AmazonCreateKerridgeLog(IKerridgeService kerridgeService)
        {
            _kerridgeService = kerridgeService;
        }

        public int Order { get { return 101; } }
        public void OnAmazonOrderPlaced(AmazonOrder order)
        {
            var orderMrCms = order.Order;

            if (orderMrCms.PaymentStatus.Equals(PaymentStatus.Paid) && !orderMrCms.IsCancelled && order.PurchaseDate > new DateTime(2013, 10, 7, 6, 0, 0) && order.FulfillmentChannel.Equals(AmazonFulfillmentChannel.MFN))
            {
                _kerridgeService.Add(new KerridgeLog
                    {
                        Order = orderMrCms,
                        Sent = false
                    });
            }
        }
    }
}