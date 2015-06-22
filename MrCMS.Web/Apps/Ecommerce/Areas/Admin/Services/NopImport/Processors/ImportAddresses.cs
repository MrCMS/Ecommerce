using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportAddresses : IImportAddresses
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public ImportAddresses(IStatelessSession session,Site site)
        {
            _session = session;
            _site = site;
        }

        public string ProcessAddresses(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            var addressData = dataReader.GetAddressData();
            var site = _session.Get<Site>(_site.Id);
            _session.Transact(session =>
            {
                foreach (var data in addressData)
                {
                    var address = new Address
                    {
                        Address1 = data.Address1,
                        Address2 = data.Address2,
                        City = data.City,
                        Company = data.Company,
                        CountryCode = data.CountryCode,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        PhoneNumber = data.PhoneNumber,
                        PostalCode = data.PostalCode,
                        StateProvince = data.StateProvince,
                    };
                    address.AssignBaseProperties(site);
                    session.Insert(address);
                    nopImportContext.AddEntry(data.Id, address);
                }
            });
            return string.Format("{0} addresses added", addressData.Count);
        }
    }
}