using System;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class CartItem : SiteEntity
    {
        public virtual ICanAddToCart Item { get; set; }
        public virtual Guid UserGuid { get; set; }

        public virtual int Quantity { get; set; }

        public virtual decimal Price
        {
            get { return Item.Price * Quantity; }
        }

        public virtual decimal Saving
        {
            get { return Item.ReducedBy * Quantity; }
        }

        public virtual decimal Tax
        {
            get { return Item.Tax * Quantity; }
        }

        public virtual bool CurrentlyAvailable
        {
            get { return Item.CanBuy(Quantity); }
        }

        public virtual decimal PricePreTax
        {
            get { return Item.PricePreTax * Quantity; }
        }

        public virtual decimal TaxRatePercentage
        {
            get { return Item.TaxRatePercentage; }
        }
    }
}