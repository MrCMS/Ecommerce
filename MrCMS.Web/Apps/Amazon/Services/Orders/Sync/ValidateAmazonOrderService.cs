using System;
using System.Linq;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using NHibernate;
using Address = MrCMS.Web.Apps.Ecommerce.Entities.Users.Address;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ValidateAmazonOrderService : IValidateAmazonOrderService
    {
        private readonly ICountryService _countryService;
        private readonly ISession _session;

        public ValidateAmazonOrderService(ICountryService countryService, ISession session)
        {
            _countryService = countryService;
            _session = session;
        }

        public void SetAmazonOrderItem(ref AmazonOrder amazonOrder, OrderItem rawOrderItem)
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
        private AmazonOrderItem GetAmazonOrderItem(AmazonOrder amazonOrder, OrderItem rawOrderItem)
        {
            return new AmazonOrderItem()
                {
                    ASIN = rawOrderItem.ASIN,
                    AmazonOrder = amazonOrder,
                    AmazonOrderItemId = rawOrderItem.OrderItemId,
                    Title = rawOrderItem.Title,
                    SellerSKU = rawOrderItem.SellerSKU,
                    Condition = rawOrderItem.ConditionId.GetEnumByValue<AmazonListingCondition>(),
                    ConditionSubtype =
                        rawOrderItem.ConditionSubtypeId.GetEnumByValue<AmazonListingCondition>(),

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

                    ShippingDiscountAmount = Decimal.Parse(rawOrderItem.ShippingDiscount.Amount),
                    ShippingDiscountCurrency = rawOrderItem.ShippingDiscount.CurrencyCode,
                    ShippingPriceAmount = Decimal.Parse(rawOrderItem.ShippingPrice.Amount),
                    ShippingPriceCurrency = rawOrderItem.ShippingPrice.CurrencyCode,
                    ShippingTaxAmount = Decimal.Parse(rawOrderItem.ShippingTax.Amount),
                    ShippingTaxCurrency = rawOrderItem.ShippingTax.CurrencyCode
                };
        }

        public void GetAmazonOrderDetails(MarketplaceWebServiceOrders.Model.Order rawOrder, ref AmazonOrder order, Address shippingAddress)
        {
            order.AmazonOrderId = rawOrder.AmazonOrderId;
            order.BuyerEmail = rawOrder.BuyerEmail;
            order.BuyerName = rawOrder.BuyerName;
            order.PurchaseDate = rawOrder.PurchaseDate;
            order.PaymentMethod = rawOrder.PaymentMethod.GetEnumByValue<AmazonPaymentMethod>();
            order.SalesChannel = rawOrder.SalesChannel;
            order.OrderType = rawOrder.OrderType;
            order.OrderTotalAmount = rawOrder.OrderTotal != null ? Decimal.Parse(rawOrder.OrderTotal.Amount) : 0;
            order.OrderCurrency = rawOrder.OrderTotal != null ? rawOrder.OrderTotal.CurrencyCode : String.Empty;
            order.MarketplaceId = rawOrder.MarketplaceId;
            order.ShipServiceLevel = rawOrder.ShipServiceLevel;
            order.ShipmentServiceLevelCategory = rawOrder.ShipmentServiceLevelCategory;
            order.ShippingAddress = shippingAddress;
            order.NumberOfItemsUnshipped = rawOrder.NumberOfItemsUnshipped;
            order.NumberOfItemsShipped = rawOrder.NumberOfItemsShipped;
            order.FulfillmentChannel = rawOrder.FulfillmentChannel.GetEnumByValue<AmazonFulfillmentChannel>();
            order.LastUpdatedDate = rawOrder.LastUpdateDate;
        }
        public Address GetAmazonOrderAddress(MarketplaceWebServiceOrders.Model.Order rawOrder)
        {
            if (rawOrder.ShippingAddress != null)
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
            return null;
        }

        public Order GetOrder(MarketplaceWebServiceOrders.Model.Order rawOrder, AmazonOrder amazonOrder)
        {
            var order = amazonOrder.Order ?? new Order();

            if (order.Id == 0)
            {
                order = GetOrderDetails(amazonOrder);

                GetOrderLines(amazonOrder, ref order);
            }

            order.ShippingStatus = rawOrder.OrderStatus.GetEnumByValue<ShippingStatus>();

            return order;
        }
        private Order GetOrderDetails(AmazonOrder amazonOrder)
        {
            return new Order()
                {
                    ShippingAddress = amazonOrder.ShippingAddress.ToAddressData(_session),
                    BillingAddress = amazonOrder.ShippingAddress.ToAddressData(_session),
                    PaymentMethod = amazonOrder.PaymentMethod.GetDescription(),
                    PaidDate = amazonOrder.PurchaseDate,
                    Total = amazonOrder.OrderTotalAmount,
                    Tax = amazonOrder.Tax,
                    Subtotal = amazonOrder.ItemAmount,
                    DiscountAmount = amazonOrder.ItemDiscountAmount,
                    ShippingTax = amazonOrder.ShippingTax,
                    ShippingTotal = amazonOrder.ShippingTotal,
                    OrderEmail = amazonOrder.BuyerEmail,
                    IsCancelled = false,
                    SalesChannel = "Amazon"
                };
        }
        private void GetOrderLines(AmazonOrder amazonOrder, ref Order order)
        {
            foreach (var amazonOrderItem in amazonOrder.Items)
            {
                order.OrderLines.Add(new OrderLine()
                    {
                        Order = order,
                        UnitPrice = amazonOrderItem.ItemPriceAmount,
                        Price = amazonOrderItem.ItemPriceAmount * amazonOrderItem.QuantityOrdered,
                        Name = amazonOrderItem.Title,
                        Tax = amazonOrderItem.ItemTaxAmount,
                        Discount = amazonOrderItem.PromotionDiscountAmount,
                        Quantity = Decimal.ToInt32(amazonOrderItem.QuantityOrdered),
                        SKU = amazonOrderItem.SellerSKU
                    });
            }
        }
    }
}