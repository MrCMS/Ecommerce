using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Discounts
{
    public class Discount : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }

        public virtual string ValidTimePeriod
        {
            get 
            {
                if (DateFrom == null && DateTo == null)
                    return "Forever";
                else if (DateFrom != null && DateTo == null)
                    return "From " + DateFrom;
                else if (DateFrom == null && DateTo != null)
                    return "Until " + DateTo;
                else
                    return DateFrom + " - " + DateTo;
            }
        }

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
                                                      Application != null && Application.Unproxy().GetType() == type,
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

        public virtual decimal GetDiscount(CartItem cartItem, string discountCode)
        {
            if (!IsCodeValid(discountCode))
                return 0m;

            if (Limitation != null)
                if (!Limitation.IsItemValid(cartItem))
                    return 0m;

            return Application == null ? 0m : Application.GetDiscount(cartItem);
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