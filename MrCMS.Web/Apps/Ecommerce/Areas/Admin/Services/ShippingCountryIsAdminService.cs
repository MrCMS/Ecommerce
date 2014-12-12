using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class ShippingCountryIsAdminService : IShippingCountryIsAdminService
    {
        private readonly ISession _session;

        public ShippingCountryIsAdminService(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetCountryOptions()
        {
            return _session.Query<Country>().OrderBy(country => country.Name).Cacheable()
                .BuildSelectItemList(country => country.Name, country => country.ISOTwoLetterCode,
                    emptyItemText: "Please select...");
        }
    }
}