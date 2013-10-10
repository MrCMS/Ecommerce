using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Entities;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using System;
using MrCMS.Entities.People;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Orders
{
    public class Order : SiteEntity, IBelongToUser
    {
        public Order()
        {
            OrderLines = new List<OrderLine>();
            OrderNotes = new List<OrderNote>();
            OrderRefunds = new List<OrderRefund>();
            Guid = Guid.NewGuid();
        }

        public virtual Guid Guid { get; set; }
        public virtual decimal Subtotal { get; set; }

        public virtual decimal Tax { get; set; }
        public virtual decimal Total { get; set; }

        [DisplayName("Total after Refunds")]
        public virtual decimal TotalAfterRefunds
        {
            get
            {
                return Total - TotalRefunds;
            }
        }

        public virtual decimal TotalRefunds
        {
            get
            {
                return OrderRefunds.Sum(item => item.Amount);
            }
        }

        public virtual decimal DiscountAmount { get; set; }
        public virtual Discount Discount { get; set; }
        [DisplayName("Discount Code")]
        public virtual string DiscountCode { get; set; }

        public virtual decimal Weight { get; set; }
        [DisplayName("Shipping Method")]
        public virtual ShippingMethod ShippingMethod { get; set; }
        [DisplayName("Shipping Total")]
        public virtual decimal? ShippingTotal { get; set; }
        [DisplayName("Shipping Tax")]
        public virtual decimal? ShippingTax { get; set; }
        [DisplayName("Shipping Tax Percentage")]
        public virtual decimal? ShippingTaxPercentage { get; set; }
        [DisplayName("Shipping Status")]
        public virtual ShippingStatus ShippingStatus { get; set; }
        [DisplayName("Shipping Date")]
        public virtual DateTime? ShippingDate { get; set; }

        public virtual AddressData ShippingAddress { get; set; }
        public virtual AddressData BillingAddress { get; set; }

        [DisplayName("Payment Method")]
        public virtual string PaymentMethod { get; set; }
        [DisplayName("Payment Status")]
        public virtual PaymentStatus PaymentStatus { get; set; }
        [DisplayName("Payment Date")]
        public virtual DateTime? PaidDate { get; set; }

        [DisplayName("Customer IP")]
        public virtual string CustomerIP { get; set; }
        public virtual User User { get; set; }
        [DisplayName("Order Email")]
        public virtual string OrderEmail { get; set; }

        public virtual IList<OrderLine> OrderLines { get; set; }
        public virtual IList<OrderNote> OrderNotes { get; set; }
        public virtual IList<OrderRefund> OrderRefunds { get; set; }

        public virtual string AuthorisationToken { get; set; }
        public virtual string CaptureTransactionId { get; set; }

        [DisplayName("Is Cancelled")]
        public virtual bool IsCancelled { get; set; }

        [DisplayName("Tracking Number")]
        [StringLength(250)]
        public virtual string TrackingNumber { get; set; }

        [DisplayName("Sales Channel")]
        public virtual string SalesChannel { get; set; }
    }

    public class AddressData : IAddress
    {
        public virtual string Name
        {
            get { return string.Format("{0} {1} {2}", Title, FirstName, LastName); }
        }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [DisplayName("County")]
        public string StateProvince { get; set; }
        public Country Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }

        public string GetDescription(bool removeName = false)
        {
            var addressParts = GetAddressParts(removeName);
            return string.Join(", ", addressParts);
        }

        private IEnumerable<string> GetAddressParts(bool removeName)
        {
            if (!string.IsNullOrWhiteSpace(Name) && !removeName)
                yield return Name;
            if (!string.IsNullOrWhiteSpace(Company))
                yield return Company;
            if (!string.IsNullOrWhiteSpace(Address1))
                yield return Address1;
            if (!string.IsNullOrWhiteSpace(Address2))
                yield return Address2;
            if (!string.IsNullOrWhiteSpace(City))
                yield return City;
            if (!string.IsNullOrWhiteSpace(StateProvince))
                yield return StateProvince;
            if (Country != null)
                yield return Country.Name;
            if (!string.IsNullOrWhiteSpace(PostalCode))
                yield return PostalCode;
        }
    }
}