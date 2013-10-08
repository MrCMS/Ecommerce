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
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class ValidateAmazonOrderService : IValidateAmazonOrderService
    {
        private readonly ICountryService _countryService;
        private readonly EcommerceSettings _ecommerceSettings;

        public ValidateAmazonOrderService(ICountryService countryService, EcommerceSettings ecommerceSettings)
        {
            _countryService = countryService;
            _ecommerceSettings = ecommerceSettings;
        }

        public void SetAmazonOrderItem(ref AmazonOrder amazonOrder, OrderItem rawOrderItem)
        {
            if (amazonOrder.Items.SingleOrDefault(x => x.AmazonOrderItemId == rawOrderItem.OrderItemId) != null)
                amazonOrder.Items.SingleOrDefault(x => x.AmazonOrderItemId == rawOrderItem.OrderItemId).QuantityShipped = rawOrderItem.QuantityShipped;
            else
            {
                var orderItem = GetAmazonOrderItem(amazonOrder, rawOrderItem);

                amazonOrder.Items.Add(orderItem);
            }
        }
        private AmazonOrderItem GetAmazonOrderItem(AmazonOrder amazonOrder, OrderItem rawOrderItem)
        {
            return new AmazonOrderItem
                       {
                    ASIN = !String.IsNullOrWhiteSpace(rawOrderItem.ASIN)?rawOrderItem.ASIN:String.Empty,
                    AmazonOrder = amazonOrder,
                    AmazonOrderItemId = !String.IsNullOrWhiteSpace(rawOrderItem.OrderItemId) ? rawOrderItem.OrderItemId : String.Empty,
                    Title = !String.IsNullOrWhiteSpace(rawOrderItem.Title) ? rawOrderItem.Title : String.Empty,
                    SellerSKU = !String.IsNullOrWhiteSpace(rawOrderItem.SellerSKU) ? rawOrderItem.SellerSKU : String.Empty,
                    Condition = !String.IsNullOrWhiteSpace(rawOrderItem.ConditionId) ? rawOrderItem.ConditionId.GetEnumByValue<AmazonListingCondition>() : AmazonListingCondition.New,
                    ConditionSubtype = !String.IsNullOrWhiteSpace(rawOrderItem.ConditionSubtypeId) ? rawOrderItem.ConditionSubtypeId.GetEnumByValue<AmazonListingCondition>() : AmazonListingCondition.New,

                    GiftWrapPriceAmount = (rawOrderItem.GiftWrapPrice!=null && rawOrderItem.GiftWrapPrice.Amount!=null)?Decimal.Parse(rawOrderItem.GiftWrapPrice.Amount):0,
                    GiftWrapPriceCurrency = (rawOrderItem.GiftWrapPrice != null && rawOrderItem.GiftWrapPrice.CurrencyCode != null) ? rawOrderItem.GiftWrapPrice.CurrencyCode : _ecommerceSettings.Currency.Code,
                    GiftWrapTaxAmount = (rawOrderItem.GiftWrapTax != null && rawOrderItem.GiftWrapTax.Amount != null)?Decimal.Parse(rawOrderItem.GiftWrapTax.Amount):0,
                    GiftWrapTaxCurrency = (rawOrderItem.GiftWrapTax != null && rawOrderItem.GiftWrapTax.CurrencyCode != null) ? rawOrderItem.GiftWrapTax.CurrencyCode : _ecommerceSettings.Currency.Code,

                    ItemPriceAmount = (rawOrderItem.ItemPrice != null && rawOrderItem.ItemPrice.Amount != null) ? Decimal.Parse(rawOrderItem.ItemPrice.Amount) : 0,
                    ItemPriceCurrency = (rawOrderItem.ItemPrice != null && rawOrderItem.ItemPrice.CurrencyCode != null) ? rawOrderItem.ItemPrice.CurrencyCode : _ecommerceSettings.Currency.Code,
                    ItemTaxAmount = (rawOrderItem.ItemTax != null && rawOrderItem.ItemTax.Amount != null) ? Decimal.Parse(rawOrderItem.ItemTax.Amount) : 0,
                    ItemTaxCurrency = (rawOrderItem.ItemTax != null && rawOrderItem.ItemTax.CurrencyCode != null) ? rawOrderItem.ItemTax.CurrencyCode : _ecommerceSettings.Currency.Code,

                    PromotionDiscountAmount =  (rawOrderItem.PromotionDiscount != null && rawOrderItem.PromotionDiscount.Amount != null) ? Decimal.Parse(rawOrderItem.PromotionDiscount.Amount) : 0,
                    PromotionDiscountCurrency = (rawOrderItem.PromotionDiscount != null && rawOrderItem.PromotionDiscount.CurrencyCode != null) ? rawOrderItem.PromotionDiscount.CurrencyCode : _ecommerceSettings.Currency.Code,

                    QuantityOrdered = rawOrderItem.QuantityOrdered,
                    QuantityShipped = rawOrderItem.QuantityShipped,

                    ShippingDiscountAmount = (rawOrderItem.ShippingDiscount != null && rawOrderItem.ShippingDiscount.Amount != null) ? Decimal.Parse(rawOrderItem.ShippingDiscount.Amount) : 0,
                    ShippingDiscountCurrency = (rawOrderItem.ShippingDiscount != null && rawOrderItem.ShippingDiscount.CurrencyCode != null) ? rawOrderItem.ShippingDiscount.CurrencyCode : _ecommerceSettings.Currency.Code,
                    ShippingPriceAmount = (rawOrderItem.ShippingPrice != null && rawOrderItem.ShippingPrice.Amount != null) ? Decimal.Parse(rawOrderItem.ShippingPrice.Amount) : 0,
                    ShippingPriceCurrency = (rawOrderItem.ShippingPrice != null && rawOrderItem.ShippingPrice.CurrencyCode != null) ? rawOrderItem.ShippingPrice.CurrencyCode : _ecommerceSettings.Currency.Code,
                    ShippingTaxAmount = (rawOrderItem.ShippingTax != null && rawOrderItem.ShippingTax.Amount != null) ? Decimal.Parse(rawOrderItem.ShippingTax.Amount) : 0,
                    ShippingTaxCurrency = (rawOrderItem.ShippingTax != null && rawOrderItem.ShippingTax.CurrencyCode != null) ? rawOrderItem.ShippingTax.CurrencyCode : _ecommerceSettings.Currency.Code,
                };
        }

        public void GetAmazonOrderDetails(MarketplaceWebServiceOrders.Model.Order rawOrder, ref AmazonOrder order, AddressData shippingAddress)
        {
            order.AmazonOrderId = !String.IsNullOrWhiteSpace(rawOrder.AmazonOrderId) ? rawOrder.AmazonOrderId : String.Empty;
            order.BuyerEmail = !String.IsNullOrWhiteSpace(rawOrder.BuyerEmail) ? rawOrder.BuyerEmail : String.Empty;
            order.BuyerName = !String.IsNullOrWhiteSpace(rawOrder.BuyerName) ? rawOrder.BuyerName : String.Empty;
            order.PurchaseDate = rawOrder.PurchaseDate;
            order.PaymentMethod = rawOrder.PaymentMethod.GetEnumByValue<AmazonPaymentMethod>();
            order.SalesChannel = !String.IsNullOrWhiteSpace(rawOrder.SalesChannel) ? rawOrder.SalesChannel : String.Empty;
            order.OrderType = !String.IsNullOrWhiteSpace(rawOrder.OrderType) ? rawOrder.OrderType : String.Empty;
            order.OrderTotalAmount = (rawOrder.OrderTotal != null && rawOrder.OrderTotal.Amount!=null) ? Decimal.Parse(rawOrder.OrderTotal.Amount) : 0;
            order.OrderCurrency = (rawOrder.OrderTotal != null && rawOrder.OrderTotal.Amount != null) ? rawOrder.OrderTotal.CurrencyCode : String.Empty;
            order.MarketplaceId = !String.IsNullOrWhiteSpace(rawOrder.MarketplaceId) ? rawOrder.MarketplaceId : String.Empty;
            order.ShipServiceLevel = !String.IsNullOrWhiteSpace(rawOrder.ShipServiceLevel) ? rawOrder.ShipServiceLevel : String.Empty;
            order.ShipmentServiceLevelCategory = !String.IsNullOrWhiteSpace(rawOrder.ShipmentServiceLevelCategory) ? rawOrder.ShipmentServiceLevelCategory : String.Empty;
            SetShippingAddress(order, shippingAddress);
            order.NumberOfItemsUnshipped = rawOrder.NumberOfItemsUnshipped;
            order.NumberOfItemsShipped = rawOrder.NumberOfItemsShipped;
            order.FulfillmentChannel = rawOrder.FulfillmentChannel.GetEnumByValue<AmazonFulfillmentChannel>();
            order.LastUpdatedDate = rawOrder.LastUpdateDate;
        }

        public void SetShippingAddress(AmazonOrder amazonOrder, AddressData address)
        {
            amazonOrder.ShippingAddress = address;
        }

        public AddressData GetAmazonOrderAddress(MarketplaceWebServiceOrders.Model.Order rawOrder)
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
                               Country =
                                   (rawOrder.ShippingAddress != null &&
                                    !String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.CountryCode))
                                       ? _countryService.GetCountryByCode(rawOrder.ShippingAddress.CountryCode)
                                       : null,
                           };
            }
            return null;
        }

        private static void GetFirstAndLastName(MarketplaceWebServiceOrders.Model.Order rawOrder, out string firstName, out string lastName)
        {
            firstName = String.Empty;
            lastName = String.Empty;
            try
            {
                if (rawOrder.ShippingAddress == null || String.IsNullOrWhiteSpace(rawOrder.ShippingAddress.Name))
                    return;
                var rawName = rawOrder.ShippingAddress.Name.Split(' ');
                if (rawName.Any())
                    firstName = rawName[0];
                if (rawName.Count() >= 2)
                    lastName = rawName[1];
            }
            catch (Exception)
            {
                firstName = String.Empty;
                lastName = String.Empty;
            }
        }

        public Order GetOrder(AmazonOrder amazonOrder)
        {
            var order = amazonOrder.Order ?? new Order();

            if (order.Id == 0)
            {
                order = GetOrderDetails(amazonOrder);

                GetOrderLines(amazonOrder, ref order);
            }

            order.ShippingStatus = amazonOrder.Status.GetEnumByValue<ShippingStatus>();

            return order;
        }
        private Order GetOrderDetails(AmazonOrder amazonOrder)
        {
            return new Order
                       {
                           ShippingAddress = amazonOrder.ShippingAddress,
                           BillingAddress = amazonOrder.ShippingAddress,
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
                           SalesChannel = "Amazon",
                           PaymentStatus = PaymentStatus.Paid,
                           CreatedOn = amazonOrder.PurchaseDate.HasValue ? (DateTime)amazonOrder.PurchaseDate : CurrentRequestData.Now
                       };
        }
        private void GetOrderLines(AmazonOrder amazonOrder, ref Order order)
        {
            foreach (var amazonOrderItem in amazonOrder.Items)
            {
                order.OrderLines.Add(new OrderLine()
                    {
                        Order = order,
                        UnitPrice = amazonOrderItem.QuantityOrdered>0?(amazonOrderItem.ItemPriceAmount / amazonOrderItem.QuantityOrdered):0,
                        Price = amazonOrderItem.ItemPriceAmount,
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