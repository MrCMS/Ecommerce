using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Shipping
{
    public class ShippingMethod : SiteEntity
    {
        public ShippingMethod()
        {
            ShippingCalculations = new List<ShippingCalculation>();
            Orders = new List<Order>();
            ExcludedProductVariants = new List<ProductVariant>();
        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<ShippingCalculation> ShippingCalculations { get; set; }
        public virtual IList<Order> Orders { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual TaxRate TaxRate { get; set; }

        public virtual string TaxRateName
        {
            get { return TaxRate == null ? string.Empty : TaxRate.Name; }
        }

        public virtual decimal TaxRatePercentage
        {
            get { return TaxRate == null ? 0m : TaxRate.Percentage; }
        }

        public virtual IList<ProductVariant> ExcludedProductVariants { get; set; }

        public virtual bool CanBeUsed(CartModel model)
        {
            return ShippingCalculations.Any(calculation => calculation.CanBeUsed(model));
        }

        public virtual decimal? GetPrice(CartModel model)
        {
            if (!CanBeUsed(model))
                return null;

            ShippingCalculation shippingCalculation = GetCheapestShippingCalculation(model);

            if (model.Country != null)
                shippingCalculation = ShippingCalculations
                    .Where(calculation => calculation.CanBeUsed(model)
                                          && calculation.Country.Id == model.Country.Id)
                    .OrderBy(calculation => calculation.GetPrice(model))
                    .FirstOrDefault();

            return shippingCalculation != null
                       ? shippingCalculation.GetPrice(model)
                       : null;
        }

        public virtual decimal? GetTax(CartModel model)
        {
            if (!CanBeUsed(model))
                return null;

            ShippingCalculation shippingCalculation = GetCheapestShippingCalculation(model);

            return shippingCalculation != null
                       ? shippingCalculation.GetTax(model)
                       : null;
        }

        public virtual ShippingCalculation GetCheapestShippingCalculation(CartModel model)
        {
            return
                ShippingCalculations.Where(calculation => calculation.CanBeUsed(model))
                                    .OrderBy(calculation => calculation.GetPrice(model))
                                    .FirstOrDefault();
        }
    }
}