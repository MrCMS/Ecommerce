using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public class NopImportContext
    {
        public NopImportContext()
        {
            Entries = new HashSet<NopImportContextEntry>();
        }

        public HashSet<NopImportContextEntry> Entries { get; private set; }

        public void AddEntry<T>(int id, T newEntity) where T : SystemEntity
        {
            Entries.Add(new NopImportContextEntry
            {
                Id = id,
                Type = typeof(T),
                NewEntity = newEntity
            });
        }

        public T FindNew<T>(int id) where T : SystemEntity
        {
            NopImportContextEntry entry = Entries.FirstOrDefault(x => typeof(T).IsAssignableFrom(x.Type) && x.Id == id);
            if (entry != null) return entry.NewEntity as T;
            return null;
        }
    }
}