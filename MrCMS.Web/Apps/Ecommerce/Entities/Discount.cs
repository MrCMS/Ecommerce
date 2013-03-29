using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class Discount : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }

        [DisplayName("Date From")]
        public virtual DateTime? DateFrom { get; set; }
        [DisplayName("Date To")]
        public virtual DateTime? DateTo { get; set; }

        public virtual DiscountLimitation Limitation { get; set; }
        public virtual DiscountApplication Application { get; set; }

        public virtual List<SelectListItem> LimitationOptions
        {
            get
            {
                return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<DiscountLimitation>()
                                 .BuildSelectItemList(type => type.Name.BreakUpString(), type => type.FullName,
                                                      type =>
                                                      Limitation != null && Limitation.Unproxy().GetType() == type,
                                                      "No limitations");
            }
        }
        public virtual List<SelectListItem> ApplicationOptions
        {
            get
            {
                return TypeHelper.GetAllConcreteMappedClassesAssignableFrom<DiscountApplication>()
                                 .BuildSelectItemList(type => type.Name.BreakUpString(), type => type.FullName,
                                                      type =>
                                                      Limitation != null && Application.Unproxy().GetType() == type,
                                                      emptyItemText: null);
            }
        }

        public virtual decimal GetDiscount(CartModel cartModel)
        {
            if (!IsCodeValid(cartModel.DiscountCode))
                return 0m;

            if (Limitation != null)
                if (!Limitation.IsCartValid(cartModel))
                    return 0m;

            return Application == null ? 0m : Application.GetDiscount(cartModel);
        }

        private bool IsCodeValid(string discountCode)
        {
            if (!string.Equals(Name, discountCode, StringComparison.OrdinalIgnoreCase))
                return false;

            if (DateFrom.HasValue && DateFrom > DateTime.UtcNow)
                return false;
            
            if (DateTo.HasValue && DateTo > DateTime.UtcNow)
                return false;

            return true;
        }
    }
}