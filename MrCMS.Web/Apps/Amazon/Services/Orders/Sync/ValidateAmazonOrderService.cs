using System;
using System.Globalization;
using System.Linq;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;
using Order = MarketplaceWebServiceOrders.Model.Order;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ValidateAmazonOrderService : IValidateAmazonOrderService
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly ISession _session;
        private readonly ISetTaxes _setTax;

        public ValidateAmazonOrderService(EcommerceSettings ecommerceSettings, ISetTaxes setTax, ISession session)
        {
            _ecommerceSettings = ecommerceSettings;
            _setTax = setTax;
            _session = session;
        }

        public void SetAmazonOrderItem(ref AmazonOrder amazonOrder, OrderItem rawOrderItem)
        {
            AmazonOrderItem item =
                amazonOrder.Items.SingleOrDefault(x => x.AmazonOrderItemId == rawOrderItem.OrderItemId);
            if (item != null)
                item.QuantityShipped = rawOrderItem.QuantityShipped;
            else
            {
                AmazonOrderItem orderItem = GetAmazonOrderItem(amazonOrder, rawOrderItem);

                amazonOrder.Items.Add(orderItem);
            }
        }

        public void GetAmazonOrderDetails(Order rawOrder, ref AmazonOrder order, AddressData shippingAddress)
        {
            order.AmazonOrderId = !String.IsNullOrWhiteSpace(rawOrder.AmazonOrderId)
                ? rawOrder.AmazonOrderId
                : String.Empty;
            order.BuyerEmail = !String.IsNullOrWhiteSpace(rawOrder.BuyerEmail) ? rawOrder.BuyerEmail : String.Empty;
            order.BuyerName = !String.IsNullOrWhiteSpace(rawOrder.BuyerName) ? rawOrder.BuyerName : String.Empty;
            order.PurchaseDate = TimeZoneInfo.ConvertTime(rawOrder.PurchaseDate, TimeZoneInfo.Utc,
                CurrentRequestData.TimeZoneInfo);
            order.PaymentMethod = rawOrder.PaymentMethod.GetEnumByValue<AmazonPaymentMethod>();
            order.SalesChannel = !String.IsNullOrWhiteSpace(rawOrder.SalesChannel)
                ? rawOrder.SalesChannel
                : String.Empty;
            order.OrderType = !String.IsNullOrWhiteSpace(rawOrder.OrderType) ? rawOrder.OrderType : String.Empty;
            order.OrderTotalAmount = (rawOrder.OrderTotal != null && rawOrder.OrderTotal.Amount != null)
                ? Decimal.Parse(rawOrder.OrderTotal.Amount, (new CultureInfo("en-GB", false)))
                : 0;
            order.OrderCurrency = (rawOrder.OrderTotal != null && rawOrder.OrderTotal.Amount != null)
                ? rawOrder.OrderTotal.CurrencyCode
                : String.Empty;
            order.MarketplaceId = !String.IsNullOrWhiteSpace(rawOrder.MarketplaceId)
                ? rawOrder.MarketplaceId
                : String.Empty;
            order.ShipServiceLevel = !String.IsNullOrWhiteSpace(rawOrder.ShipServiceLevel)
                ? rawOrder.ShipServiceLevel
                : String.Empty;
            order.ShipmentServiceLevelCategory = !String.IsNullOrWhiteSpace(rawOrder.ShipmentServiceLevelCategory)
                ? rawOrder.ShipmentServiceLevelCategory
                : String.Empty;
            SetShippingAddress(order, shippingAddress);
            order.NumberOfItemsUnshipped = rawOrder.NumberOfItemsUnshipped;
            order.NumberOfItemsShipped = rawOrder.NumberOfItemsShipped;
            order.FulfillmentChannel = rawOrder.FulfillmentChannel.GetEnumByValue<AmazonFulfillmentChannel>();
            order.LastUpdatedDate = TimeZoneInfo.ConvertTime(rawOrder.LastUpdateDate, TimeZoneInfo.Utc,
                CurrentRequestData.TimeZoneInfo);
        }

        public void SetShippingAddress(AmazonOrder amazonOrder, AddressData address)
        {
            amazonOrder.ShippingAddress = address;
        }

        public AddressData GetAmazonOrderAddress(Order rawOrder)
        {
            if (rawOrder.ShippingAddress != null)
            {
                string firstName;
                string lastName;
                GetFirstAndLastName(rawOrder, out firstName, out lastName);

                return new AddressData
                {
                    Address1 =
                        (rawOrder.ShippingAddress != null &&
                         !String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.AddressLine1))
                            ? rawOrder.ShippingAddress.AddressLine1
                            : String.Empty,
                    Address2 =
                        (rawOrder.ShippingAddress != null &&
                         !String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.AddressLine2))
                            ? rawOrder.ShippingAddress.AddressLine2
                            : String.Empty,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber =
                        (rawOrder.ShippingAddress != null &&
                         !String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.Phone))
                            ? rawOrder.ShippingAddress.Phone
                            : String.Empty,
                    PostalCode =
                        (rawOrder.ShippingAddress != null &&
                         !String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.PostalCode))
                            ? rawOrder.ShippingAddress.PostalCode
                            : String.Empty,
                    StateProvince =
                        (rawOrder.ShippingAddress != null &&
                         !String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.StateOrRegion))
                            ? rawOrder.ShippingAddress.StateOrRegion
                            : String.Empty,
                    City =
                        (rawOrder.ShippingAddress != null &&
                         !String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.City))
                            ? rawOrder.ShippingAddress.City
                            : String.Empty,
                    CountryCode = (rawOrder.ShippingAddress != null &&
                                   !String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.CountryCode))
                        ? rawOrder.ShippingAddress.CountryCode
                        : String.Empty,
                };
            }
            return null;
        }

        public Ecommerce.Entities.Orders.Order GetOrder(AmazonOrder amazonOrder)
        {
            Ecommerce.Entities.Orders.Order order = amazonOrder.Order ?? new Ecommerce.Entities.Orders.Order();

            if (order.Id == 0)
            {
                order = GetOrderDetails(amazonOrder);
                Ecommerce.Entities.Orders.Order order1 = order;
                _session.Transact(session => session.Save(order1));
                GetOrderLines(amazonOrder, ref order);

                _setTax.SetTax(ref order, amazonOrder.Tax);
            }

            order.ShippingStatus = amazonOrder.Status.GetEnumByValue<ShippingStatus>();

            return order;
        }

        private AmazonOrderItem GetAmazonOrderItem(AmazonOrder amazonOrder, OrderItem rawOrderItem)
        {
            Currency currency = _ecommerceSettings.Currency();
            return new AmazonOrderItem
            {
                ASIN = !String.IsNullOrWhiteSpace(rawOrderItem.ASIN) ? rawOrderItem.ASIN : String.Empty,
                AmazonOrder = amazonOrder,
                AmazonOrderItemId =
                    !String.IsNullOrWhiteSpace(rawOrderItem.OrderItemId) ? rawOrderItem.OrderItemId : String.Empty,
                Title = !String.IsNullOrWhiteSpace(rawOrderItem.Title) ? rawOrderItem.Title : String.Empty,
                SellerSKU =
                    !String.IsNullOrWhiteSpace(rawOrderItem.SellerSKU) ? rawOrderItem.SellerSKU : String.Empty,
                Condition =
                    !String.IsNullOrWhiteSpace(rawOrderItem.ConditionId)
                        ? rawOrderItem.ConditionId.GetEnumByValue<AmazonListingCondition>()
                        : AmazonListingCondition.New,
                ConditionSubtype =
                    !String.IsNullOrWhiteSpace(rawOrderItem.ConditionSubtypeId)
                        ? rawOrderItem.ConditionSubtypeId.GetEnumByValue<AmazonListingCondition>()
                        : AmazonListingCondition.New,
                GiftWrapPriceAmount =
                    (rawOrderItem.GiftWrapPrice != null && rawOrderItem.GiftWrapPrice.Amount != null)
                        ? Decimal.Parse(rawOrderItem.GiftWrapPrice.Amount, new CultureInfo("en-GB", false))
                        : 0,
                GiftWrapPriceCurrency =
                    (rawOrderItem.GiftWrapPrice != null && rawOrderItem.GiftWrapPrice.CurrencyCode != null)
                        ? rawOrderItem.GiftWrapPrice.CurrencyCode
                        : currency.Code,
                GiftWrapTaxAmount =
                    (rawOrderItem.GiftWrapTax != null && rawOrderItem.GiftWrapTax.Amount != null)
                        ? Decimal.Parse(rawOrderItem.GiftWrapTax.Amount, new CultureInfo("en-GB", false))
                        : 0,
                GiftWrapTaxCurrency =
                    (rawOrderItem.GiftWrapTax != null && rawOrderItem.GiftWrapTax.CurrencyCode != null)
                        ? rawOrderItem.GiftWrapTax.CurrencyCode
                        : currency.Code,
                ItemPriceAmount =
                    (rawOrderItem.ItemPrice != null && rawOrderItem.ItemPrice.Amount != null)
                        ? Decimal.Parse(rawOrderItem.ItemPrice.Amount, new CultureInfo("en-GB", false))
                        : 0,
                ItemPriceCurrency =
                    (rawOrderItem.ItemPrice != null && rawOrderItem.ItemPrice.CurrencyCode != null)
                        ? rawOrderItem.ItemPrice.CurrencyCode
                        : currency.Code,
                ItemTaxAmount =
                    (rawOrderItem.ItemTax != null && rawOrderItem.ItemTax.Amount != null)
                        ? Decimal.Parse(rawOrderItem.ItemTax.Amount, new CultureInfo("en-GB", false))
                        : 0,
                ItemTaxCurrency =
                    (rawOrderItem.ItemTax != null && rawOrderItem.ItemTax.CurrencyCode != null)
                        ? rawOrderItem.ItemTax.CurrencyCode
                        : currency.Code,
                PromotionDiscountAmount =
                    (rawOrderItem.PromotionDiscount != null && rawOrderItem.PromotionDiscount.Amount != null)
                        ? Decimal.Parse(rawOrderItem.PromotionDiscount.Amount, new CultureInfo("en-GB", false))
                        : 0,
                PromotionDiscountCurrency =
                    (rawOrderItem.PromotionDiscount != null && rawOrderItem.PromotionDiscount.CurrencyCode != null)
                        ? rawOrderItem.PromotionDiscount.CurrencyCode
                        : currency.Code,
                QuantityOrdered = rawOrderItem.QuantityOrdered,
                QuantityShipped = rawOrderItem.QuantityShipped,
                ShippingDiscountAmount =
                    (rawOrderItem.ShippingDiscount != null && rawOrderItem.ShippingDiscount.Amount != null)
                        ? Decimal.Parse(rawOrderItem.ShippingDiscount.Amount, new CultureInfo("en-GB", false))
                        : 0,
                ShippingDiscountCurrency =
                    (rawOrderItem.ShippingDiscount != null && rawOrderItem.ShippingDiscount.CurrencyCode != null)
                        ? rawOrderItem.ShippingDiscount.CurrencyCode
                        : currency.Code,
                ShippingPriceAmount =
                    (rawOrderItem.ShippingPrice != null && rawOrderItem.ShippingPrice.Amount != null)
                        ? Decimal.Parse(rawOrderItem.ShippingPrice.Amount, new CultureInfo("en-GB", false))
                        : 0,
                ShippingPriceCurrency =
                    (rawOrderItem.ShippingPrice != null && rawOrderItem.ShippingPrice.CurrencyCode != null)
                        ? rawOrderItem.ShippingPrice.CurrencyCode
                        : currency.Code,
                ShippingTaxAmount =
                    (rawOrderItem.ShippingTax != null && rawOrderItem.ShippingTax.Amount != null)
                        ? Decimal.Parse(rawOrderItem.ShippingTax.Amount, new CultureInfo("en-GB", false))
                        : 0,
                ShippingTaxCurrency =
                    (rawOrderItem.ShippingTax != null && rawOrderItem.ShippingTax.CurrencyCode != null)
                        ? rawOrderItem.ShippingTax.CurrencyCode
                        : currency.Code,
            };
        }

        private static void GetFirstAndLastName(Order rawOrder, out string firstName, out string lastName)
        {
            firstName = String.Empty;
            lastName = String.Empty;
            try
            {
                if (rawOrder.ShippingAddress == null || String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.Name))
                    return;
                string[] rawName = rawOrder.ShippingAddress.Name.Split(' ');
                if (rawName.Any())
                    firstName = rawName[0];

                if (rawName.Count() < 2) return;

                for (int i = 1; i < rawName.Count(); i++)
                {
                    lastName += rawName[i] + " ";
                }
            }
            catch (Exception)
            {
                firstName = String.Empty;
                lastName = String.Empty;
            }
        }

        private Ecommerce.Entities.Orders.Order GetOrderDetails(AmazonOrder amazonOrder)
        {
            return new Ecommerce.Entities.Orders.Order
            {
                ShippingAddress = amazonOrder.ShippingAddress,
                BillingAddress = amazonOrder.ShippingAddress,
                PaymentMethod = amazonOrder.PaymentMethod.GetDescription(),
                PaidDate = amazonOrder.PurchaseDate,
                Total = amazonOrder.OrderTotalAmount,
                TotalPaid = amazonOrder.OrderTotalAmount,
                Subtotal = amazonOrder.ItemAmount,
                DiscountAmount = amazonOrder.ItemDiscountAmount,
                ShippingTax = amazonOrder.ShippingTax,
                ShippingTotal = amazonOrder.ShippingTotal,
                OrderEmail = amazonOrder.BuyerEmail,
                IsCancelled = false,
                SalesChannel = AmazonApp.SalesChannel,
                PaymentStatus = PaymentStatus.Paid,
                OrderDate = amazonOrder.PurchaseDate.HasValue ? amazonOrder.PurchaseDate : CurrentRequestData.Now
            };
        }

        private void GetOrderLines(AmazonOrder amazonOrder, ref Ecommerce.Entities.Orders.Order order)
        {
            foreach (AmazonOrderItem amazonOrderItem in amazonOrder.Items)
            {
                var orderLine = new OrderLine
                {
                    Order = order,
                    UnitPrice =
                        amazonOrderItem.QuantityOrdered > 0
                            ? (amazonOrderItem.ItemPriceAmount/amazonOrderItem.QuantityOrdered)
                            : 0,
                    Price = amazonOrderItem.ItemPriceAmount,
                    Name = amazonOrderItem.Title,
                    Tax = amazonOrderItem.ItemTaxAmount,
                    Discount = amazonOrderItem.PromotionDiscountAmount,
                    Quantity = Decimal.ToInt32(amazonOrderItem.QuantityOrdered),
                    SKU = amazonOrderItem.SellerSKU
                };
                order.OrderLines.Add(orderLine);
                _session.Transact(session => session.Save(orderLine));
            }
        }
    }
}