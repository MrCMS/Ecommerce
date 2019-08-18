using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class OnlineCustomerCartItem
    {
        public ProductVariant Product { get; set; }
        public int Quantity { get; set; }
    }
    public class OnlineCustomerCart
    {
        public int? UserId { get; set; }
        public Guid? UserGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public bool BillingAddressIsSameAsShipping { get; set; }
        public List<OnlineCustomerCartItem> Items { get; set; }
        public int ItemsCount { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime LastUpdatedOn { get; set; }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
                {
                    return "Guest";
                }
                return FirstName + " " + LastName;
            }
        }

        public string BillingAddressDescription
        {
            get
            {
                return BillingAddressIsSameAsShipping
                    ? "Same as Shipping Address"
                    : BillingAddress == null
                        ? "N/A"
                        : BillingAddress.GetDescription();
            }
        }
    }
}