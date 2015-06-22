using System;
using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public class NopImportContext
    {
        public NopImportContext()
        {
            Entries = new Dictionary<Type, Dictionary<int, object>>();
        }

        public Dictionary<Type, Dictionary<int, object>> Entries { get; private set; }


        public void AddEntry<T>(int id, T newEntity) where T : SystemEntity
        {
            Type type = typeof (T);
            if (!Entries.ContainsKey(type))
                Entries[type] = new Dictionary<int, object>();

            Entries[type][id] = newEntity;
        }

        public T FindNew<T>(int id) where T : SystemEntity
        {
            Type type = typeof (T);
            if (!Entries.ContainsKey(type) || !Entries[type].ContainsKey(id))
                return default(T);
            return Entries[type][id] as T;
        }
    }
}