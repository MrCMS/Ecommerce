using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;
namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class DiscountLimitationService : IDiscountLimitationService
    {
        private readonly ISession _session;

        public DiscountLimitationService(ISession session)
        {
            _session = session;
        }

        public DiscountLimitation Get(int id)
        {
            return _session.QueryOver<DiscountLimitation>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }
    }
}