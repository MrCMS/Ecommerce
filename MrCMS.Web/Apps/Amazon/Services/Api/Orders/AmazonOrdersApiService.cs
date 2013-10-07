using System;
using System.Collections.Generic;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;
using GetServiceStatusRequest = MarketplaceWebServiceOrders.Model.GetServiceStatusRequest;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Orders
{
    public class AmazonOrdersApiService : IAmazonOrdersApiService
    {
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly MarketplaceWebServiceOrders.MarketplaceWebServiceOrders _marketplaceWebServiceOrders;

        public AmazonOrdersApiService(AmazonSellerSettings amazonSellerSettings,
            IAmazonAnalyticsService amazonAnalyticsService, IAmazonLogService amazonLogService, MarketplaceWebServiceOrders.MarketplaceWebServiceOrders marketplaceWebServiceOrders)
        {
            _amazonSellerSettings = amazonSellerSettings;
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonLogService = amazonLogService;
            _marketplaceWebServiceOrders = marketplaceWebServiceOrders;
        }

        private AmazonServiceStatus GetServiceStatus(AmazonApiSection apiSection)
        {
            try
            {
                var ordersApiRequest = new GetServiceStatusRequest { SellerId = _amazonSellerSettings.SellerId };
                var ordersApiResult = _marketplaceWebServiceOrders.GetServiceStatus(ordersApiRequest);
                if (ordersApiResult != null && ordersApiResult.GetServiceStatusResult != null)
                    return ordersApiResult.GetServiceStatusResult.Status.GetEnumByValue<AmazonServiceStatus>();

            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, apiSection, "GetServiceStatus", null, null, null);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return AmazonServiceStatus.RED;
        }
        public bool IsLive(AmazonApiSection apiSection)
        {
            var serviceStatus = GetServiceStatus(apiSection);
            return serviceStatus == AmazonServiceStatus.GREEN || serviceStatus == AmazonServiceStatus.GREEN_I;
        }

        public IEnumerable<Order> ListOrders(AmazonSyncModel model)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, null, null, AmazonApiSection.Orders,
                    "ListOrders", null, null, null, "Listing Amazon Orders");
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrders");
                var request = GetListOrdersRequest(model);

                var result = _marketplaceWebServiceOrders.ListOrders(request);

                if (result != null && result.ListOrdersResult != null && result.IsSetListOrdersResult() && result.ListOrdersResult.Orders.Order != null)
                    return result.ListOrdersResult.Orders.Order;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Orders,
                    "ListOrders", null, null, null, "Error happend during operation of listing Amazon Orders");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        private ListOrdersRequest GetListOrdersRequest(AmazonSyncModel model)
        {
            var marketplace = new MarketplaceIdList();
            var request = new ListOrdersRequest
                {
                    SellerId = _amazonSellerSettings.SellerId,
                    MarketplaceId = marketplace.WithId(_amazonSellerSettings.MarketplaceId),
                };
            if (model.From.HasValue)
                request.CreatedAfter = model.From.Value;
            if (model.To.HasValue)
                request.CreatedBefore = model.To.Value;
            return request;
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

                if (result != null && result.ListOrderItemsResult != null && result.IsSetListOrderItemsResult()
                    && result.ListOrderItemsResult.OrderItems != null)
                    return result.ListOrderItemsResult.OrderItems.OrderItem;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null,
                    AmazonApiSection.Orders, "ListOrderItems", null, null, null,
                    "Error happend during operation of listing items for Amazon Order #" + amazonOrderId);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        private ListOrderItemsRequest GetListOrderItemsRequest(string amazonOrderId)
        {
            return new ListOrderItemsRequest
            {
                SellerId = _amazonSellerSettings.SellerId,
                AmazonOrderId = amazonOrderId
            };
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

            return GetOrders(request);
        }
        public List<Order> ListUpdatedOrders(GetUpdatedOrdersRequest newOrdersRequest)
        {
            var request = new ListOrdersRequest
            {
                SellerId = _amazonSellerSettings.SellerId,
                MarketplaceId = new MarketplaceIdList().WithId(_amazonSellerSettings.MarketplaceId),
                LastUpdatedAfter = newOrdersRequest.LastUpdatedAfter,
                LastUpdatedBefore = newOrdersRequest.LastUpdatedBefore,
            };

            return GetOrders(request);
        }
        private List<Order> GetOrders(ListOrdersRequest request)
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
    }
}