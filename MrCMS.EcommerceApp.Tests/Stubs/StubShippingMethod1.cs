using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.EcommerceApp.Tests.Stubs
{
    public class StubShippingMethod1 : IShippingMethod
    {
        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public string DisplayName
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Description
        {
            get { throw new System.NotImplementedException(); }
        }

        public string TypeName
        {
            get { return GetType().FullName; }
        }

        public bool CanBeUsed(CartModel cart)
        {
            throw new System.NotImplementedException();
        }

        public bool CanPotentiallyBeUsed(CartModel cart)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetShippingTotal(CartModel cart)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetShippingTax(CartModel cart)
        {
            throw new System.NotImplementedException();
        }

        public decimal TaxRatePercentage
        {
            get { throw new System.NotImplementedException(); }
        }

        public string ConfigureAction
        {
            get { throw new System.NotImplementedException(); }
        }

        public string ConfigureController
        {
            get { throw new System.NotImplementedException(); }
        }
    }
    public class StubShippingMethod2 : IShippingMethod
    {
        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public string DisplayName
        {
            get { throw new System.NotImplementedException(); }
        }

        public string Description
        {
            get { throw new System.NotImplementedException(); }
        }

        public string TypeName
        {
            get { return GetType().FullName; }
        }

        public bool CanBeUsed(CartModel cart)
        {
            throw new System.NotImplementedException();
        }

        public bool CanPotentiallyBeUsed(CartModel cart)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetShippingTotal(CartModel cart)
        {
            throw new System.NotImplementedException();
        }

        public decimal GetShippingTax(CartModel cart)
        {
            throw new System.NotImplementedException();
        }

        public decimal TaxRatePercentage
        {
            get { throw new System.NotImplementedException(); }
        }

        public string ConfigureAction
        {
            get { throw new System.NotImplementedException(); }
        }

        public string ConfigureController
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}