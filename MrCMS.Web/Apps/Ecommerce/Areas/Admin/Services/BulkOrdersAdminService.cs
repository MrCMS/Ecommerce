using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class BulkOrdersAdminService : IBulkOrdersAdminService
    {
        private readonly IOrderService _orderService;

        public BulkOrdersAdminService(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public MarkOrdersAsShippedViewModel GetModel(SelectedOrdersViewModel model)
        {
            var orderIds = model.Orders.Split(',').Select(int.Parse).ToList();
            var shippedModel = new MarkOrdersAsShippedViewModel { Page = model.Page };
            if (!orderIds.Any()) return shippedModel;
            foreach (var orderId in orderIds)
                shippedModel.Orders.Add(_orderService.Get(orderId));
            return shippedModel;
        }

        public List<PickingListViewModel> GetPickingList(SelectedOrdersViewModel model)
        {
            if (!model.Orders.Any())
                return new List<PickingListViewModel>();

            var orderIds = model.Orders.Split(',').Select(int.Parse).ToList();
            var orders = new List<Order>();
            if (orderIds.Any())
                orders.AddRange(orderIds.Select(orderId => _orderService.Get(orderId)));

            var pickingList = new List<PickingListViewModel>();
            foreach (var order in orders)
            {
                foreach (var orderline in order.OrderLines)
                {
                    var alreadyExists = pickingList.Any(x => x.ProductVariantId == orderline.ProductVariant.Id);
                    if (alreadyExists)
                    {
                        var pickingListItem = pickingList.SingleOrDefault(x => x.ProductVariantId == orderline.ProductVariant.Id);
                        if (pickingListItem != null) pickingListItem.Quantity += orderline.Quantity;
                    }
                    else
                    {
                        pickingList.Add(new PickingListViewModel
                        {
                            ProductVariantId = orderline.ProductVariant.Id,
                            ProductName = orderline.ProductVariant.DisplayName,
                            Sku = orderline.ProductVariant.SKU,
                            Quantity = orderline.Quantity
                        });
                    }
                }
            }
            return pickingList.OrderByDescending(x => x.Quantity).ToList();
        }
    }

    public static class BulkOrdersHelper
    {
        public static List<SelectListItem> GetShippingMethodPerOrder(this int orderId)
        {
            var order = MrCMSApplication.Get<IOrderAdminService>().Get(orderId);
            var shippingMethods = MrCMSApplication.Get<IShippingMethodAdminService>().GetAll()
                .Where(info => info.Enabled);
            if (order == null)
                return shippingMethods.BuildSelectItemList(info => info.DisplayName, info => info.Name, emptyItem: null);

            if (string.IsNullOrWhiteSpace(order.ShippingMethodName))
                return shippingMethods.BuildSelectItemList(info => info.DisplayName, info => info.Name, emptyItem: null);

            var shippingMethodInfo = shippingMethods.SingleOrDefault(x => x.Name.ToLower() == order.ShippingMethodName.ToLower());

            if(shippingMethodInfo == null)
                return shippingMethods.BuildSelectItemList(info => info.DisplayName, info => info.Name, emptyItem: null);

            return shippingMethods
                    .BuildSelectItemList(info => info.DisplayName, info => info.Name, info => info.Name == order.ShippingMethodName, emptyItem: null);
        }
    }
}