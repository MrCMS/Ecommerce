using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportAddresses : IImportAddresses
    {
        private readonly ISession _session;

        public ImportAddresses(ISession session)
        {
            _session = session;
        }

        public string ProcessAddresses(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            var addressData = dataReader.GetAddressData();
            _session.Transact(session =>
            {
                foreach (var data in addressData)
                {
                    var country = nopImportContext.FindNew<Country>(data.Country.GetValueOrDefault());
                    var address = new Address
                    {
                        Address1 = data.Address1,
                        Address2 = data.Address2,
                        City = data.City,
                        Company = data.Company,
                        CountryCode = country == null ? string.Empty : country.ISOTwoLetterCode,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        PhoneNumber = data.PhoneNumber,
                        PostalCode = data.PostalCode,
                        StateProvince = data.StateProvince,
                    };

                    session.Save(address);
                    nopImportContext.AddEntry(data.Id, address);
                }
            });
            return string.Format("{0} addresses added", addressData.Count);
        }
    }
}