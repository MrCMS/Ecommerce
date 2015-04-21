using System;
using System.Collections.Generic;
using System.Linq;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Orders
{
    public class AmazonOrdersApiService : IAmazonOrdersApiService
    {
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly MarketplaceWebServiceOrders.MarketplaceWebServiceOrders _marketplaceWebServiceOrders;

        public AmazonOrdersApiService(AmazonSellerSettings amazonSellerSettings,
            IAmazonAnalyticsService amazonAnalyticsService, IAmazonLogService amazonLogService,
            MarketplaceWebServiceOrders.MarketplaceWebServiceOrders marketplaceWebServiceOrders)
        {
            _amazonSellerSettings = amazonSellerSettings;
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonLogService = amazonLogService;
            _marketplaceWebServiceOrders = marketplaceWebServiceOrders;
        }

        public List<Order> ListSpecificOrders(IEnumerable<string> orderIds)
        {
            var request = new GetOrderRequest
            {
                SellerId = _amazonSellerSettings.SellerId,
                AmazonOrderId = new OrderIdList().WithId(orderIds.Select(x => x).ToArray())
            };

            return GetOrder(request);
        }
        private List<Order> GetOrder(GetOrderRequest request)
        {
            _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, null, null, AmazonApiSection.Orders,
                    "GetOrder", null, null, null, "Listing Specific Amazon Orders (" + request.AmazonOrderId.Id.ToTokenizedString(", ") + ")");
            _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "GetOrder");
            var result = _marketplaceWebServiceOrders.GetOrder(request);
            var orders = new List<Order>();
            if (result == null || !result.IsSetGetOrderResult())
                return orders;
            if (result.GetOrderResult.IsSetOrders() && result.GetOrderResult.Orders.IsSetOrder())
                orders.AddRange(result.GetOrderResult.Orders.Order);

            return orders;
        }

        public List<Order> ListCreatedOrders(GetUpdatedOrdersRequest updatedOrdersRequest)
        {
            var request = new ListOrdersRequest
            {
                SellerId = _amazonSellerSettings.SellerId,
                MarketplaceId = new MarketplaceIdList().WithId(_amazonSellerSettings.MarketplaceId),
                CreatedAfter = updatedOrdersRequest.LastUpdatedAfter,
                CreatedBefore = updatedOrdersRequest.LastUpdatedBefore,
            };

            return ListOrders(request);
        }
        public IEnumerable<Order> ListUpdatedOrders(GetUpdatedOrdersRequest newOrdersRequest)
        {
            var request = new ListOrdersRequest
            {
                SellerId = _amazonSellerSettings.SellerId,
                MarketplaceId = new MarketplaceIdList().WithId(_amazonSellerSettings.MarketplaceId),
                LastUpdatedAfter = newOrdersRequest.LastUpdatedAfter,
                LastUpdatedBefore = newOrdersRequest.LastUpdatedBefore,
            };

            return ListOrders(request);
        }
        private List<Order> ListOrders(ListOrdersRequest request)
        {
            _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, null, null, AmazonApiSection.Orders,
                   "ListOrders", null, null, null, "Listing Amazon Orders");
            _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrders");
            var result = _marketplaceWebServiceOrders.ListOrders(request);
            var orders = new List<Order>();
            if (result == null || !result.IsSetListOrdersResult())
                return orders;
            if (result.ListOrdersResult.IsSetOrders() && result.ListOrdersResult.Orders.IsSetOrder())
                orders.AddRange(result.ListOrdersResult.Orders.Order);

            var nextToken = result.ListOrdersResult.NextToken;
            while (!string.IsNullOrWhiteSpace(nextToken))
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, null, null, AmazonApiSection.Orders,
                   "ListOrdersByNextToken", null, null, null, "Listing Amazon Orders (Next Token)");
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrdersByNextToken");
                var response = _marketplaceWebServiceOrders.ListOrdersByNextToken(new ListOrdersByNextTokenRequest
                                                                                      {
                                                                                          SellerId = _amazonSellerSettings.SellerId,
                                                                                          NextToken = result.ListOrdersResult.NextToken
                                                                                      });
                if (response != null && response.IsSetListOrdersByNextTokenResult())
                {
                    if (response.ListOrdersByNextTokenResult.IsSetOrders() && response.ListOrdersByNextTokenResult.Orders.IsSetOrder())
                        orders.AddRange(response.ListOrdersByNextTokenResult.Orders.Order);
                    nextToken = response.ListOrdersByNextTokenResult.NextToken;
                }
                else nextToken = null;
            }

            return orders;
        }

        public IEnumerable<OrderItem> ListOrderItems(string amazonOrderId)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, null, null, AmazonApiSection.Orders, "ListOrderItems",
                    null, null, null, "Listing items for Amazon Order #" + amazonOrderId);
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrderItems");
                var request = GetListOrderItemsRequest(amazonOrderId);

                var result = _marketplaceWebServiceOrders.ListOrderItems(request);

                return result.ListOrderItemsResult.OrderItems.OrderItem;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null,
                    AmazonApiSection.Orders, "ListOrderItems", null, null, null,
                    "Error happend during operation of listing items for Amazon Order #" + amazonOrderId);
                throw;
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                throw;
            }
        }
        private ListOrderItemsRequest GetListOrderItemsRequest(string amazonOrderId)
        {
            return new ListOrderItemsRequest
            {
                SellerId = _amazonSellerSettings.SellerId,
                AmazonOrderId = amazonOrderId
            };
        }
    }
}