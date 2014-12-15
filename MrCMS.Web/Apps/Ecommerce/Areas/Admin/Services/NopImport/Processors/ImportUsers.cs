using System.Text;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
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
                        var address = nopImportContext.FindNew<Address>(addressData.Id);
                        if (address != null)
                        {
                            address.User = user;
                            session.Update(address);
                        }
                    }
                }
            });

            return string.Format("{0} users imported.", userDatas.Count);
        }
    }
}