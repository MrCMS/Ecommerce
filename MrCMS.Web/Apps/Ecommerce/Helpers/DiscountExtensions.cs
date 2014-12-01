using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class DiscountExtensions
    {
        public static string ValidTimePeriod(this Discount discount)
        {
            if (discount.ValidFrom == null && discount.ValidUntil == null)
                return "Forever";
            if (discount.ValidFrom != null && discount.ValidUntil == null)
                return String.Format("From {0}", discount.ValidFrom);
            if (discount.ValidFrom == null && discount.ValidUntil != null)
                return String.Format("Until {0}", discount.ValidUntil);
            return String.Format("{0} - {1}", discount.ValidFrom, discount.ValidUntil);
        }

        public static List<SelectListItem> GetLimitationOptions(this Discount discount)
        {
            return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<DiscountLimitation>()
                .BuildSelectItemList(type => type.Name.BreakUpString(), type => type.FullName,
                    type => discount.Limitation != null && discount.Limitation.Unproxy().GetType() == type,
                    "No limitations");
        }

        public static List<SelectListItem> GetApplicationOptions(this Discount discount)
        {
            return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<DiscountApplication>()
                .BuildSelectItemList(type => type.Name.BreakUpString(), type => type.FullName,
                    type => discount.Application != null && discount.Application.Unproxy().GetType() == type,
                    emptyItemText: null);
        }
    }
}