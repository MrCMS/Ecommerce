using System.Text;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportUsers
    {
        string ProcessUsers(NopCommerceDataReader dataReader, NopImportContext nopImportContext);
    }

    public class ImportUsers : IImportUsers
    {
        private readonly ISession _session;

        public ImportUsers(ISession session)
        {
            _session = session;
        }

        public string ProcessUsers(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            var userDatas = dataReader.GetUserData();
            _session.Transact(session =>
            {

                foreach (var userData in userDatas)
                {
                    var user = new User
                    {
                        CurrentEncryption = userData.Format,
                        PasswordHash = Encoding.UTF8.GetBytes(userData.Hash),
                        PasswordSalt = Encoding.UTF8.GetBytes(userData.Salt),
                        FirstName = userData.FirstName,
                        LastName = userData.LastName,
                        Email = userData.Email,
                        IsActive = userData.Active,
                        Guid = userData.Guid
                    };
                    session.Save(user);
                    foreach (var addressData in userData.AddressData)
                    {
                        var country = nopImportContext.FindNew<Country>(addressData.Country.GetValueOrDefault());
                        var address = new Address
                        {
                            Address1 = addressData.Address1,
                            Address2 = addressData.Address2,
                            City = addressData.City,
                            Company = addressData.Company,
                            CountryCode = country == null ? string.Empty : country.ISOTwoLetterCode,
                            FirstName = addressData.FirstName,
                            LastName = addressData.LastName,
                            PhoneNumber = addressData.PhoneNumber,
                            PostalCode = addressData.PostalCode,
                            StateProvince = addressData.StateProvince,
                            User = user
                        };

                        session.Save(address);
                    }
                }
            });

            return string.Format("{0} users imported.", userDatas.Count);
        }
    }
}