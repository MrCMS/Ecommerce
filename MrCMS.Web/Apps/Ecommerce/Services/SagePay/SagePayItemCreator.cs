using System;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Ecommerce.Helpers.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public class SagePayItemCreator : ISagePayItemCreator
    {
        private readonly Site _site;

        public SagePayItemCreator(Site site)
        {
            _site = site;
        }

        public ShoppingBasket GetShoppingBasket(CartModel model)
        {
            var shoppingBasket = new ShoppingBasket(string.Format("{0} Shopping Basket", _site.Name));

            foreach (var item in model.Items)
            {
                shoppingBasket.Add(new BasketItem
                                       {
                                           Description = item.Name,
                                           ItemPrice = item.UnitPricePreTax,
                                           ItemTax = item.UnitTax,
                                           ItemTotal = item.UnitPrice,
                                           LineTotal = item.Price,
                                           Quantity = item.Quantity
                                       });
                if (item.DiscountAmount > 0)
                {
                    shoppingBasket.Add(new BasketItem
                                           {
                                               Description = "Discount for - " + item.Name,
                                               LineTotal = item.DiscountAmount,
                                           });
                }
            }

            if (model.HasOrderTotalDiscount())
            {
                shoppingBasket.Add(new BasketItem
                                       {
                                           Description = "Order Discount - " + model.DiscountCodes,
                                           LineTotal = model.OrderTotalDiscount,
                                       });
            }

            if (model.ShippingTotal > 0)
            {
                shoppingBasket.Add(new BasketItem
                {
                    Description = "Shipping - " + model.ShippingMethod.Name,
                    ItemPrice = model.ShippingPreTax,
                    ItemTax = model.ShippingTax,
                    ItemTotal = model.ShippingTotal,
                    LineTotal = model.ShippingTotal,
                });
            }

            return shoppingBasket;
        }

        public Address GetAddress(Entities.Users.Address address)
        {
            return address == null
                       ? new Address()
                       : new Address
                             {
                                 Address1 = address.Address1,
                                 Address2 = address.Address2,
                                 City = address.City,
                                 Country = address.CountryCode,
                                 Firstnames = address.FirstName,
                                 Phone = address.PhoneNumber,
                                 PostCode = address.PostalCode,
                                 Surname = address.LastName
                             };
        }
    }
}