using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models
{
    public class UserData
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Hash { get; set; }

        public string Salt { get; set; }

        public string Format { get; set; }

        public Guid Guid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public HashSet<AddressData> AddressData { get; set; }

        public bool Active { get; set; }
    }
}