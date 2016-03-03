using System;
using System.Collections.Generic;
using System.Text;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportUsers : IImportUsers
    {
        private readonly IStatelessSession _session;

        public ImportUsers(IStatelessSession session)
        {
            _session = session;
        }

        public string ProcessUsers(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            HashSet<UserData> userDatas = dataReader.GetUserData();
            var guids = _session.QueryOver<User>().Select(x=>x.Guid).List<Guid>();
            _session.Transact(session =>
            {
                foreach (UserData userData in userDatas)
                {
                    var guid = userData.Guid;
                    if (guids.Contains(guid))
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
                    };
                    user.SetGuid(userData.Guid);
                    user.AssignBaseProperties();
                    session.Insert(user);
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