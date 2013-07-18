using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using NHibernate;
using MrCMS.Helpers;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Payments
{
    public class PaymentStatusService : IPaymentStatusService
    {        public List<SelectListItem> GetOptions()
        {
            return
                Enum.GetValues(typeof (PaymentStatus))
                    .Cast<PaymentStatus>()
                    .BuildSelectItemList(item => item.ToString(), emptyItem: null);
        }
    }
}