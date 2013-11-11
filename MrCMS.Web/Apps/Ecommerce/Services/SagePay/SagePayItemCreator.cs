using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Ecommerce.Models;
using SagePayMvc;

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
                shoppingBasket.Add(new BasketItem(item.Quantity, item.Name, item.PricePreTax,
                                                  1 + (item.TaxRatePercentage / 100m)));

            if (model.HasDiscount)
                shoppingBasket.Add(new BasketItem(1, "Discount - " + model.DiscountCode, model.DiscountAmount, 1));

            if (model.ShippingTotal.GetValueOrDefault() > 0)
                shoppingBasket.Add(new BasketItem(1, "Shipping - " + model.ShippingMethod.Name,
                                                  model.ShippingPreTax.GetValueOrDefault(),
                                                  1 + (model.ShippingTaxPercentage.GetValueOrDefault() / 100m)));

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