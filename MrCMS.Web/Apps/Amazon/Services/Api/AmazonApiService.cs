using System;
using System.Collections.Generic;
using MarketplaceWebServiceOrders;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Analytics;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Api
{
    public class AmazonApiService : IAmazonApiService
    {
        private readonly AmazonAppSettings _amazonAppSettings;
        private readonly AmazonSellerSettings _amazonSellerSettings;

        private readonly IAmazonAnalyticsService _amazonAnalyticsService;
        private readonly IAmazonLogService _amazonLogService;

        public AmazonApiService(AmazonAppSettings amazonAppSettings, 
            AmazonSellerSettings amazonSellerSettings, 
            IAmazonAnalyticsService amazonAnalyticsService, IAmazonLogService amazonLogService)
        {
            _amazonAppSettings = amazonAppSettings;
            _amazonSellerSettings = amazonSellerSettings;
            _amazonAnalyticsService = amazonAnalyticsService;
            _amazonLogService = amazonLogService;
        }

        #region Api

        private MarketplaceWebServiceOrdersClient GetOrdersApiService()
        {
            var config = new MarketplaceWebServiceOrdersConfig() { ServiceURL = _amazonAppSettings.GetOrdersApiEndpoint };
            return new MarketplaceWebServiceOrdersClient("MrCMS", MrCMSApplication.AssemblyVersion, _amazonAppSettings.AWSAccessKeyId,
                                                         _amazonAppSettings.SecretKey, config);
        }
        public ServiceStatusEnum GetServiceStatus(AmazonApiSection apiSection)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(apiSection, "GetServiceStatus");
                switch (apiSection)
                {
                    case AmazonApiSection.Orders:
                        var service = GetOrdersApiService();
                        var request = new GetServiceStatusRequest {SellerId = _amazonSellerSettings.SellerId};
                        var result = service.GetServiceStatus(request);
                        if (result != null && result.GetServiceStatusResult != null)
                            return result.GetServiceStatusResult.Status;
                        break;
                    default:
                        return ServiceStatusEnum.RED;
                }
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, apiSection, "GetServiceStatus");
                return ServiceStatusEnum.RED;
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
                return ServiceStatusEnum.RED;
            }
            return ServiceStatusEnum.RED;
        }

        #endregion

        #region Orders

        public IEnumerable<Order> ListOrders(AmazonSyncModel model)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrders");
                var service = GetOrdersApiService();
                var marketPlace = new MarketplaceIdList();
                var request = new ListOrdersRequest
                {
                    SellerId = _amazonSellerSettings.SellerId,
                    MarketplaceId = marketPlace.WithId(_amazonSellerSettings.MarketplaceId),
                };
                if (model.From.HasValue)
                    request.CreatedAfter = model.From.Value;
                if (model.To.HasValue)
                    request.CreatedBefore = model.To.Value;

                var result = service.ListOrders(request);
                if (result != null && result.ListOrdersResult != null && result.IsSetListOrdersResult() && result.ListOrdersResult.Orders.Order != null)
                    return result.ListOrdersResult.Orders.Order;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Orders, "ListOrders");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }
        public IEnumerable<OrderItem> ListOrderItems(AmazonSyncModel model, string amazonOrderId)
        {
            try
            {
                _amazonAnalyticsService.TrackNewApiCall(AmazonApiSection.Orders, "ListOrderItems");
                var service = GetOrdersApiService();
                var request = new ListOrderItemsRequest
                {
                    SellerId = _amazonSellerSettings.SellerId,
                    AmazonOrderId = amazonOrderId
                };
                var result = service.ListOrderItems(request);
                if (result != null && result.ListOrderItemsResult != null && result.IsSetListOrderItemsResult() && result.ListOrderItemsResult.OrderItems != null)
                    return result.ListOrderItemsResult.OrderItems.OrderItem;
            }
            catch (MarketplaceWebServiceOrdersException ex)
            {
                _amazonLogService.Add(AmazonLogType.Api, AmazonLogStatus.Error, ex, null, AmazonApiSection.Orders, "ListOrderItems");
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }

        #endregion
    }
}