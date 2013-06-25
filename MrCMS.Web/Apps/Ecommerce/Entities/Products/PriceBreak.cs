using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using System;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class PriceBreak : SiteEntity
    {
        public virtual IBuyableItem Item { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal Price { get; set; }
        public virtual decimal GetPrice()
        {
           return Math.Round(!MrCMSApplication.Get<TaxSettings>().LoadedPricesIncludeTax
                                      ? Price
                                      : Item.TaxRatePercentage != 0
                                            ? Price * Item.TaxRatePercentage
                                            : Price, 2, MidpointRounding.AwayFromZero);
        }
    }
}