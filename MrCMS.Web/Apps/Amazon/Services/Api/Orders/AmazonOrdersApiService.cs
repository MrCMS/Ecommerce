using System;
using System.Collections.Generic;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;
using GetServiceStatusRequest = MarketplaceWebServiceOrders.Model.GetServiceStatusRequest;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Orders
{
    public class AmazonOrdersApiService : IAmazonOrdersApiService
    {
        private readonly IAmazonApiService _amazonApiService;
        private readonly AmazonSellerSettings _amazonSellerSettings;
        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly IAmazonLogService _amazonLogService;

        public AmazonOrdersApiService(AmazonSellerSettings amazonSellerSettings, 
            IAmazonAnalyticsService amazonAnalyticsService, IAmazonLogService amazonLogService, IAmazonApiService amazonApiService)
        {
            _amazonSellerSettings = amazonSellerSettings;
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonLogService = amazonLogService;
            _amazonApiService = amazonApiService;
        }

        public AmazonServiceStatus GetServiceStatus(AmazonApiSection apiSection)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(apiSection, "GetServiceStatus");

                var ordersApiService = _amazonApiService.GetOrdersApiService();
                var ordersApiRequest = new GetServiceStatusRequest { SellerId = _amazonSellerSettings.SellerId };
                var ordersApiResult = ordersApiService.GetServiceStatus(ordersApiRequest);
                if (ordersApiResult != null && ordersApiResult.GetServiceStatusResult != null)
                    return ordersApiResult.GetServiceStatusResult.Status.GetEnumByValue<AmazonServiceStatus>();

            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, apiSection, "GetServiceStatus");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return AmazonServiceStatus.RED;
        }

        public IEnumerable<Order> GetOrder(AmazonSyncModel model)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, null, "GetOrder", "Getting Amazon Orders");
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "GetOrder");
                var service = _amazonApiService.GetOrdersApiService();
                var request = GetOrderRequest(model);

                var result = service.GetOrder(request);

                if (result != null && result.GetOrderResult != null && result.IsSetGetOrderResult() && result.GetOrderResult.Orders.Order != null)
                    return result.GetOrderResult.Orders.Order;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, 
                    null, AmazonApiSection.Orders, "GetOrder","Error happened during operation of getting Amazon Orders");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        private GetOrderRequest GetOrderRequest(AmazonSyncModel model)
        {
            return new GetOrderRequest()
                {
                    SellerId = _amazonSellerSettings.SellerId,
                    AmazonOrderId = new OrderIdList().WithId(model.Description)
                };
        }

        public IEnumerable<Order> ListOrders(AmazonSyncModel model)
        {
            try
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, null, "ListOrders", "Listing Amazon Orders");
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrders");
                var service = _amazonApiService.GetOrdersApiService();
                var request = GetListOrdersRequest(model);

                var result = service.ListOrders(request);

                if (result != null && result.ListOrdersResult != null && result.IsSetListOrdersResult() && result.ListOrdersResult.Orders.Order != null)
                    return result.ListOrdersResult.Orders.Order;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, 
                    null, AmazonApiSection.Orders, "ListOrders","Error happend during operation of listing Amazon Orders");
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
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Stage, AmazonApiSection.Orders, null, 
                    null, "ListOrderItems", "Listing items for Amazon Order #"+amazonOrderId);
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrderItems");
                var service = _amazonApiService.GetOrdersApiService();
                var request = GetListOrderItemsRequest(amazonOrderId);

                var result = service.ListOrderItems(request);

                if (result != null && result.ListOrderItemsResult != null && result.IsSetListOrderItemsResult() 
                    && result.ListOrderItemsResult.OrderItems != null)
                    return result.ListOrderItemsResult.OrderItems.OrderItem;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, 
                    AmazonApiSection.Orders, "ListOrderItems","Error happend during operation of listing items for Amazon Order #"+amazonOrderId);
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
    }
}