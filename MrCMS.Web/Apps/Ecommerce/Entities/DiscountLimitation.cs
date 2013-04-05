using System.ComponentModel;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Web.Mvc;
using System.Collections.Generic;
using System;
using System.Web.Compilation;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using MrCMS.DbConfiguration.Mapping;
using FluentNHibernate.Mapping;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public abstract class DiscountLimitation : SiteEntity
    {
        public DiscountLimitation()
        {
            Discounts = new List<Discount>();
        }

        public abstract bool IsCartValid(CartModel cartModel);
        public abstract void CopyValues(DiscountLimitation limitation);

        public virtual IList<Discount> Discounts { get; set; }
    }

    public class OrderHasXOrMoreItems : DiscountLimitation
    {
        public override bool IsCartValid(CartModel cartModel)
        {
            var itemQuantity = cartModel.Items.Any() ? cartModel.Items.Sum(item => item.Quantity) : 0;
            return itemQuantity >= NumberOfItems;
        }

        public virtual decimal NumberOfItems { get; set; }

        public override void CopyValues(DiscountLimitation limitation)
        {
            this.NumberOfItems = ((OrderHasXOrMoreItems)limitation).NumberOfItems;
        }
    }

    public class OrderTotalGreaterThanX : DiscountLimitation
    {
        public override bool IsCartValid(CartModel cartModel)
        {
            return cartModel.TotalPreDiscount > OrderTotalGreaterThanValue;
        }

        [DisplayName("Value")]
        public virtual decimal OrderTotalGreaterThanValue { get; set; }

        public override void CopyValues(DiscountLimitation limitation)
        {
            this.OrderTotalGreaterThanValue = ((OrderTotalGreaterThanX)limitation).OrderTotalGreaterThanValue;
        }
    }
}