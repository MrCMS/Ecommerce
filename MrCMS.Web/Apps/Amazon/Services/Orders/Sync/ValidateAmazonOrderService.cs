using System;
using System.Collections.Generic;
using System.Linq;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Web.Apps.Ecommerce.Settings;
using Address = MrCMS.Web.Apps.Ecommerce.Entities.Users.Address;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ValidateAmazonOrderService : IValidateAmazonOrderService
    {
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly ICountryService _countryService;
        private readonly EcommerceSettings _ecommerceSettings;

        public ValidateAmazonOrderService(IAmazonOrderService amazonOrderService,
            ICountryService countryService, EcommerceSettings ecommerceSettings)
        {
            _amazonOrderService = amazonOrderService;
            _countryService = countryService;
            _ecommerceSettings = ecommerceSettings;
        }

        public AmazonOrder SetAmazonOrderItems(MarketplaceWebServiceOrders.Model.Order rawOrder, 
            IEnumerable<OrderItem> rawOrderItems, AmazonOrder amazonOrder)
        {
            foreach (var rawOrderItem in rawOrderItems)
                SetAmazonOrderItem(ref amazonOrder, rawOrderItem);

            amazonOrder.Order = GetOrder(rawOrder, amazonOrder);

            return amazonOrder;
        }

        private void SetAmazonOrderItem(ref AmazonOrder amazonOrder, OrderItem rawOrderItem)
        {
            var orderItem = amazonOrder.Items.SingleOrDefault(x => x.AmazonOrderItemId == rawOrderItem.OrderItemId);

            if (orderItem != null)
                amazonOrder.Items.SingleOrDefault(x => x.AmazonOrderItemId == rawOrderItem.OrderItemId).QuantityShipped = rawOrderItem.QuantityShipped;
            else
            {
                orderItem = GetAmazonOrderItem(amazonOrder, rawOrderItem);

                amazonOrder.Items.Add(orderItem);
            }
        }
        private static AmazonOrderItem GetAmazonOrderItem(AmazonOrder amazonOrder, OrderItem rawOrderItem)
        {
            return new AmazonOrderItem()
                {
                    ASIN = rawOrderItem.ASIN,
                    AmazonOrder = amazonOrder,
                    AmazonOrderItemId = rawOrderItem.OrderItemId,
                    Condition = rawOrderItem.ConditionId.GetEnumByValue<AmazonListingItemCondition>(),
                    ConditionSubtype =
                        rawOrderItem.ConditionSubtypeId.GetEnumByValue<AmazonListingItemCondition>(),
                    GiftWrapPriceAmount = Decimal.Parse(rawOrderItem.GiftWrapPrice.Amount),
                    GiftWrapPriceCurrency = rawOrderItem.GiftWrapPrice.CurrencyCode,
                    GiftWrapTaxAmount = Decimal.Parse(rawOrderItem.GiftWrapTax.Amount),
                    GiftWrapTaxCurrency = rawOrderItem.GiftWrapTax.CurrencyCode,
                    ItemPriceAmount = Decimal.Parse(rawOrderItem.ItemPrice.Amount),
                    ItemPriceCurrency = rawOrderItem.ItemPrice.CurrencyCode,
                    ItemTaxAmount = Decimal.Parse(rawOrderItem.ItemTax.Amount),
                    ItemTaxCurrency = rawOrderItem.ItemTax.CurrencyCode,
                    PromotionDiscountAmount = Decimal.Parse(rawOrderItem.PromotionDiscount.Amount),
                    PromotionDiscountCurrency = rawOrderItem.PromotionDiscount.CurrencyCode,
                    QuantityOrdered = rawOrderItem.QuantityOrdered,
                    QuantityShipped = rawOrderItem.QuantityShipped,
                    Title = rawOrderItem.Title,
                    SellerSKU = rawOrderItem.SellerSKU,
                    ShippingDiscountAmount = Decimal.Parse(rawOrderItem.ShippingDiscount.Amount),
                    ShippingDiscountCurrency = rawOrderItem.ShippingDiscount.CurrencyCode,
                    ShippingPriceAmount = Decimal.Parse(rawOrderItem.ShippingPrice.Amount),
                    ShippingPriceCurrency = rawOrderItem.ShippingPrice.CurrencyCode,
                    ShippingTaxAmount = Decimal.Parse(rawOrderItem.ShippingTax.Amount),
                    ShippingTaxCurrency = rawOrderItem.ShippingTax.CurrencyCode
                };
        }
        public AmazonOrder GetAmazonOrder(MarketplaceWebServiceOrders.Model.Order rawOrder)
        {
            if (rawOrder.OrderTotal.CurrencyCode == _ecommerceSettings.Currency.Code)
            {
                var order = _amazonOrderService.GetByAmazonOrderId(rawOrder.AmazonOrderId) ?? new AmazonOrder();

                if (order.Id == 0)
                {
                    var shippingAddress = GetAmazonOrderAddress(rawOrder);

                    GetAmazonOrderDetails(rawOrder, ref order, shippingAddress);
                }

                order.NumberOfItemsShipped = rawOrder.NumberOfItemsShipped;
                order.Status = rawOrder.OrderStatus.GetEnumByValue<AmazonOrderStatus>();

                return order;
            }
            return null;
        }
        private static void GetAmazonOrderDetails(MarketplaceWebServiceOrders.Model.Order rawOrder, ref AmazonOrder order, Address shippingAddress)
        {
            order.AmazonOrderId = rawOrder.AmazonOrderId;
            order.BuyerEmail = rawOrder.BuyerEmail;
            order.BuyerName = rawOrder.BuyerName;
            order.PurchaseDate = rawOrder.PurchaseDate;
            order.PaymentMethod = rawOrder.PaymentMethod.GetEnumByValue<AmazonPaymentMethod>();
            order.SalesChannel = rawOrder.SalesChannel;
            order.OrderType = rawOrder.OrderType;
            order.OrderTotalAmount = Decimal.Parse(rawOrder.OrderTotal.Amount);
            order.OrderCurrency = rawOrder.OrderTotal.CurrencyCode;
            order.MarketplaceId = rawOrder.MarketplaceId;
            order.ShipServiceLevel = rawOrder.ShipServiceLevel;
            order.ShipmentServiceLevelCategory = rawOrder.ShipmentServiceLevelCategory;
            order.ShippingAddress = shippingAddress;
            order.NumberOfItemsUnshipped = rawOrder.NumberOfItemsUnshipped;
            order.NumberOfItemsShipped = rawOrder.NumberOfItemsShipped;
            order.FulfillmentChannel = rawOrder.FulfillmentChannel.GetEnumByValue<AmazonFulfillmentChannel>();
            order.LastUpdatedDate = rawOrder.LastUpdateDate;
        }
        private Address GetAmazonOrderAddress(MarketplaceWebServiceOrders.Model.Order rawOrder)
        {
            return new Address()
                {
                    Address1 = rawOrder.ShippingAddress.AddressLine1,
                    Address2 = rawOrder.ShippingAddress.AddressLine2,
                    FirstName = rawOrder.ShippingAddress.Name.Split(' ').First(),
                    LastName = rawOrder.ShippingAddress.Name.Split(' ')[1],
                    PhoneNumber = rawOrder.ShippingAddress.Phone,
                    PostalCode = rawOrder.ShippingAddress.PostalCode,
                    StateProvince = rawOrder.ShippingAddress.StateOrRegion,
                    City = rawOrder.ShippingAddress.City,
                    Country = _countryService.GetCountryByCode(rawOrder.ShippingAddress.CountryCode),
                };
        }

        private Order GetOrder(MarketplaceWebServiceOrders.Model.Order rawOrder, AmazonOrder amazonOrder)
        {
            var order = amazonOrder.Order ?? new Order();
            if (order.Id == 0)
            {
                var address = GetOrderAddress(rawOrder);

                order = GetOrderDetails(rawOrder, address);

                GetOrderLines(amazonOrder, ref order);
            }

            order.ShippingStatus = rawOrder.OrderStatus.GetEnumByValue<ShippingStatus>();

            return order;
        }
        private static Order GetOrderDetails(MarketplaceWebServiceOrders.Model.Order rawOrder, Address address)
        {
            return new Order()
                {
                    ShippingAddress = address,
                    BillingAddress = address,
                    PaymentMethod = rawOrder.PaymentMethod.GetDescription(),
                    PaidDate = rawOrder.PurchaseDate,
                    Total = Decimal.Parse(rawOrder.OrderTotal.Amount),
                    OrderEmail = rawOrder.BuyerEmail,
                    IsCancelled = false,
                    SalesChannel = "Amazon"
                };
        }
        private Address GetOrderAddress(MarketplaceWebServiceOrders.Model.Order rawOrder)
        {
            return new Address()
                {
                    Address1 = rawOrder.ShippingAddress.AddressLine1,
                    Address2 = rawOrder.ShippingAddress.AddressLine2,
                    FirstName = rawOrder.ShippingAddress.Name.Split(' ').First(),
                    LastName = rawOrder.ShippingAddress.Name.Split(' ')[1],
                    PhoneNumber = rawOrder.ShippingAddress.Phone,
                    PostalCode = rawOrder.ShippingAddress.PostalCode,
                    StateProvince = rawOrder.ShippingAddress.StateOrRegion,
                    City = rawOrder.ShippingAddress.City,
                    Country = _countryService.GetCountryByCode(rawOrder.ShippingAddress.CountryCode),
                };
        }
        private static void GetOrderLines(AmazonOrder amazonOrder, ref Order order)
        {
            foreach (var amazonOrderItem in amazonOrder.Items)
            {
                order.OrderLines.Add(new OrderLine()
                    {
                        Order = order,
                        UnitPrice = amazonOrderItem.ItemPriceAmount,
                        Price = amazonOrderItem.ItemPriceAmount*amazonOrderItem.QuantityOrdered,
                        Name = amazonOrderItem.Title,
                        Tax = amazonOrderItem.ItemTaxAmount,
                        Discount = amazonOrderItem.PromotionDiscountAmount,
                        Quantity = Decimal.ToInt32(amazonOrderItem.QuantityOrdered),
                        SKU = amazonOrderItem.ASIN
                    });
            }
        }
    }
}