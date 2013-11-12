using MrCMS.Entities.Multisite;
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
                var vatMultiplier = 1 + (item.TaxRatePercentage / 100m);
                //shoppingBasket.Add(new BasketItem(item.Quantity, item.Name, item.Price / vatMultiplier,
                //                                  vatMultiplier));
            }

            if (model.HasDiscount)
            {
                //shoppingBasket.Add(new BasketItem(1, "Discount - " + model.DiscountCode, model.DiscountAmount, 1));
            }

            if (model.ShippingTotal.GetValueOrDefault() > 0)
            {
                var multiplier = 1 + (model.ShippingTaxPercentage.GetValueOrDefault() / 100m);
                //shoppingBasket.Add(new BasketItem(1, "Shipping - " + model.ShippingMethod.Name,
                //                                  model.ShippingTotal.GetValueOrDefault() / multiplier,
                //                                  multiplier));
            }

            return shoppingBasket;
        }

        public Address GetAddress(Entities.Users.Address address)
        {
            return new Address
                       {
                           Address1 = address.Address1,
                           Address2 = address.Address2,
                           City = address.City,
                           Country = address.Country.ISOTwoLetterCode,
                           Firstnames = address.FirstName,
                           Phone = address.PhoneNumber,
                           PostCode = address.PostalCode,
                           Surname = address.LastName
                       };
        }
    }
}