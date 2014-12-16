using System.Collections.Generic;
using System.Text;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
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
            HashSet<UserData> userDatas = dataReader.GetUserData();
            _session.Transact(session =>
            {
                foreach (UserData userData in userDatas)
                {
                    var guid = userData.Guid;
                    if (session.QueryOver<User>().Where(u => u.Guid == guid).Any())
                        continue;

                    var user = new User
                    {
                        CurrentEncryption = userData.Format,
                        PasswordHash = Encoding.Default.GetBytes(userData.Hash),
                        PasswordSalt = Encoding.Default.GetBytes(userData.Salt),
                        FirstName = userData.FirstName.LimitCharacters(255),
                        LastName = userData.LastName.LimitCharacters(255),
                        Email = userData.Email,
                        IsActive = userData.Active,
                        Guid = userData.Guid
                    };
                    session.Save(user);
                    foreach (AddressData addressData in userData.AddressData)
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