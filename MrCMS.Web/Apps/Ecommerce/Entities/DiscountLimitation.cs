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
        public abstract bool IsItemValid(CartItem cartItem);
        public abstract void CopyValues(DiscountLimitation limitation);

        public virtual IList<Discount> Discounts { get; set; }
    }
}