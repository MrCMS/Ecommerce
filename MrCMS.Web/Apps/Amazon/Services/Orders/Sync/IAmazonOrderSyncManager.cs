using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IAmazonOrderSyncManager
    {
        GetUpdatedOrdersResult GetUpdatedInfoFromAmazon(GetUpdatedOrdersRequest updatedOrdersRequest);
        GetUpdatedOrdersResult GetUpdatedInfoFromAmazonAdHoc(List<string> amazonOrderIds);
    }
    public class GetUpdatedOrdersRequest
    {
        public DateTime LastUpdatedAfter { get; set; }
        public DateTime LastUpdatedBefore { get; set; }
    }

    public class GetUpdatedOrdersResult
    {
        public string ErrorMessage { get; set; }

        public bool Success { get { return string.IsNullOrWhiteSpace(ErrorMessage); } }

        public List<AmazonOrder> OrdersUpdated { get; set; }

        public List<AmazonOrder> OrdersShipped { get; set; }
    }
}