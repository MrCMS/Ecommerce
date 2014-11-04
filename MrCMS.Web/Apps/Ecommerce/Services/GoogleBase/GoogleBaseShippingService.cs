using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseShippingService : IGoogleBaseShippingService
    {
        private readonly ISession _session;
        private readonly IShippingMethodUIService _shippingMethodUIService;

        public GoogleBaseShippingService(ISession session, IShippingMethodUIService shippingMethodUIService)
        {
            _session = session;
            _shippingMethodUIService = shippingMethodUIService;
        }

        public IEnumerable<GoogleBaseCalculationInfo> GetCheapestCalculationsForEachCountry(CartModel cart)
        {
            IList<Country> countries = _session.QueryOver<Country>().Cacheable().List();
            HashSet<IShippingMethod> shippingMethods = _shippingMethodUIService.GetEnabledMethods();
            foreach (Country country in countries)
            {
                cart.ShippingAddress = new Address
                {
                    CountryCode = country.ISOTwoLetterCode
                };
                HashSet<IShippingMethod> availableMethods =
                    shippingMethods.FindAll(method => method.CanPotentiallyBeUsed(cart));
                if (availableMethods.Any())
                {
                    IShippingMethod shippingMethod =
                        availableMethods.OrderBy(method => method.GetShippingTotal(cart)).First();
                    yield return new GoogleBaseCalculationInfo
                    {
                        CountryCode = country.ISOTwoLetterCode,
                        Price = shippingMethod.GetShippingTotal(cart),
                        ShippingMethodName = shippingMethod.DisplayName
                    };
                }
            }
        }
    }
}