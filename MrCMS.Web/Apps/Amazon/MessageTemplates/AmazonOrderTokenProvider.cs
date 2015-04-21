using System;
using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Web.Apps.Amazon.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.MessageTemplates
{
    public class AmazonOrderTokenProvider : ITokenProvider<Order>
    {
        private readonly IAmazonOrderService _amazonOrderService;

        public AmazonOrderTokenProvider(IAmazonOrderService amazonOrderService)
        {
            _amazonOrderService = amazonOrderService;
        }

        public IDictionary<string, Func<Order, string>> Tokens
        {
            get
            {
                return new Dictionary<string, Func<Order, string>>
                {
                    {
                        "AmazonOrderId", order =>
                        {
                            var byOrderId = _amazonOrderService.GetByOrderId(order.Id);
                            return byOrderId != null ? byOrderId.AmazonOrderId : string.Empty;
                        }

                    },
                    {
                        "FulfillmentChannel", order =>
                        {
                            var byOrderId = _amazonOrderService.GetByOrderId(order.Id);
                            if (byOrderId != null)
                            {
                                var fullFilmentChannel = byOrderId.FulfillmentChannel.HasValue
                                    ? byOrderId.FulfillmentChannel.Value.ToString()
                                    : string.Empty;
                                return !string.IsNullOrEmpty(fullFilmentChannel)
                                    ? fullFilmentChannel
                                    : string.Empty;
                            }
                            return string.Empty;
                        }}
                };
            }
        }
    }
}