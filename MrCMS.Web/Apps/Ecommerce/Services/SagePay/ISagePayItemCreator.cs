using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public interface ISagePayItemCreator
    {
        ShoppingBasket GetShoppingBasket(CartModel model);
        Address GetAddress(Entities.Users.Address address);
    }

    /// <summary>
    /// Item for a shopping basket
    /// </summary>
    public class BasketItem
    {
        public string Description { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal ItemTax { get; set; }
        public decimal ItemTotal { get; set; }
        public decimal LineTotal { get; set; }
        public int Quantity { get; set; }
    }

    /// <summary>
    /// A shopping basket
    /// </summary>
    public class ShoppingBasket : IEnumerable<BasketItem>
    {
        readonly List<BasketItem> basket = new List<BasketItem>();
        string name;

        /// <summary>
        /// Creates a new instance of the ShoppingBasket class
        /// </summary>
        /// <param name="name">The name of the basket (eg 'Shopping Basket for John Smith')</param>
        public ShoppingBasket(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Name of the basket
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Total cost of the basket
        /// </summary>
        public decimal Total
        {
            get { return basket.Sum(x => x.LineTotal); }
        }

        /// <summary>
        /// Adds an item to the basket
        /// </summary>
        public void Add(BasketItem item)
        {
            basket.Add(item);
        }


        /// <summary>
        /// Removes an item from the basket
        /// </summary>
        /// <param name="item"></param>
        public void Remove(BasketItem item)
        {
            basket.Remove(item);
        }

        public IEnumerator<BasketItem> GetEnumerator()
        {
            return basket.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Converts the basket to a string in a format that can be inspected by SagePay.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (basket.Count == 0)
            {
                return null;
            }

            var builder = new StringBuilder(basket.Count.ToString());

            foreach (var item in basket)
            {
                builder.Append(":");
                builder.Append(item.Description.Replace(":", "#"));
                builder.Append(":");
                builder.Append(item.Quantity);
                builder.Append(":");
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0:F2}", item.ItemPrice);
                builder.Append(":");
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0:F2}", item.ItemTax);
                builder.Append(":");
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0:F2}", item.ItemTotal);
                builder.Append(":");
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0:F2}", item.LineTotal);
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// Types of address
    /// </summary>
    public enum AddressType
    {
        Billing,
        Delivery
    }

    /// <summary>
    /// Represents a collection of fields that make up an address
    /// </summary>
    public class Address
    {
        public string Surname { get; set; }
        public string Firstnames { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }

        /// <summary>
        /// Converts the address to a string using the specified address type. The address type will become the prefix,
        /// eg using a brefix of AddressType.Billing will generate strings containing BillingSurname, BillingFirstnames etc
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string ToString(AddressType type)
        {
            string prefix = type.ToString();
            var builder = new StringBuilder();
            builder.Append(BuildPropertyString(prefix, x => x.Surname, Surname));
            builder.Append(BuildPropertyString(prefix, x => x.Firstnames, Firstnames));
            builder.Append(BuildPropertyString(prefix, x => x.Address1, Address1));
            builder.Append(BuildPropertyString(prefix, x => x.Address2, Address2, true));
            builder.Append(BuildPropertyString(prefix, x => x.City, City));
            builder.Append(BuildPropertyString(prefix, x => x.PostCode, PostCode));
            builder.Append(BuildPropertyString(prefix, x => x.Country, Country));
            builder.Append(BuildPropertyString(prefix, x => x.State, State, true));
            builder.Append(BuildPropertyString(prefix, x => x.Phone, Phone, true));

            return builder.ToString();
        }

        string BuildPropertyString(string prefix, Expression<Func<Address, object>> expression, string value, bool optional)
        {
            if (optional && string.IsNullOrEmpty(value)) return null;

            string name = PropertyToName(expression);
            return string.Format("&{0}{1}={2}", prefix, name, HttpUtility.UrlEncode(value));
        }


        string BuildPropertyString(string prefix, Expression<Func<Address, object>> expression, string value)
        {
            return BuildPropertyString(prefix, expression, value, false);
        }

        static string PropertyToName(Expression<Func<Address, object>> expression)
        {
            return (expression.Body as MemberExpression).Member.Name;
        }
    }
}