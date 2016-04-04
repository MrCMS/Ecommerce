using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration
{
    public class OrderOverride : IAutoMappingOverride<Order>
    {
        public void Override(AutoMapping<Order> mapping)
        {
            mapping.Component(order => order.BillingAddress, part =>
                                                                 {
                                                                     part.Map(data => data.Address1).Column("BillingAddress1");
                                                                     part.Map(data => data.Address2).Column("BillingAddress2");
                                                                     part.Map(data => data.City).Column("BillingCity");
                                                                     part.Map(data => data.Company).Column("BillingCompany");
                                                                     part.Map(data => data.CountryCode).Column("BillingCountryCode");
                                                                     part.Map(data => data.FirstName).Column("BillingFirstName");
                                                                     part.Map(data => data.LastName).Column("BillingLastName");
                                                                     part.Map(data => data.PhoneNumber).Column("BillingPhoneNumber");
                                                                     part.Map(data => data.PostalCode).Column("BillingPostalCode");
                                                                     part.Map(data => data.StateProvince).Column("BillingStateProvince");
                                                                     part.Map(data => data.Title).Column("BillingTitle");
                                                                 });
            mapping.Component(order => order.ShippingAddress, part =>
                                                                 {
                                                                     part.Map(data => data.Address1).Column("ShippingAddress1");
                                                                     part.Map(data => data.Address2).Column("ShippingAddress2");
                                                                     part.Map(data => data.City).Column("ShippingCity");
                                                                     part.Map(data => data.Company).Column("ShippingCompany");
                                                                     part.Map(data => data.CountryCode).Column("ShippingCountryCode");
                                                                     part.Map(data => data.FirstName).Column("ShippingFirstName");
                                                                     part.Map(data => data.LastName).Column("ShippingLastName");
                                                                     part.Map(data => data.PhoneNumber).Column("ShippingPhoneNumber");
                                                                     part.Map(data => data.PostalCode).Column("ShippingPostalCode");
                                                                     part.Map(data => data.StateProvince).Column("ShippingStateProvince");
                                                                     part.Map(data => data.Title).Column("ShippingTitle");
                                                                 });
            mapping.Map(order => order.HttpData).MakeVarCharMax();
            mapping.Map(order => order.GiftMessage).MakeVarCharMax();
            mapping.Map(x => x.SalesChannel).Index("IX_Order_SalesChannel");
            mapping.Map(x => x.AnalyticsSent).Not.Nullable().Default("1");
        }
    }
}