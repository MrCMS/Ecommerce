using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Amazon.Entities.Orders;

namespace MrCMS.Web.Apps.Amazon.DbConfiguration
{
    public class AmazonOrderOverride : IAutoMappingOverride<AmazonOrder>
    {
        public void Override(AutoMapping<AmazonOrder> mapping)
        {
            mapping.Component(order => order.ShippingAddress, part =>
            {
                part.Map(data => data.Address1).Column("ShippingAddress1");
                part.Map(data => data.Address2).Column("ShippingAddress2");
                part.Map(data => data.City).Column("ShippingCity");
                part.Map(data => data.Company).Column("ShippingCompany");
                part.References(data => data.Country).Column("ShippingCountryId");
                part.Map(data => data.FirstName).Column("ShippingFirstName");
                part.Map(data => data.LastName).Column("ShippingLastName");
                part.Map(data => data.PhoneNumber).Column("ShippingPhoneNumber");
                part.Map(data => data.PostalCode).Column("ShippingPostalCode");
                part.Map(data => data.StateProvince).Column("ShippingStateProvince");
                part.Map(data => data.Title).Column("ShippingTitle");
            });
        }
    }
}