using MrCMS.Web.Apps.Ecommerce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IDiscountLimitationService
    {
        DiscountLimitation Get(int id);
    }
}
