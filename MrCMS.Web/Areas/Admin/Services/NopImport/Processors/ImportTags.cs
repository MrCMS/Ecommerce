using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Processors
{
    public class ImportTags : IImportTags
    {
        private readonly ISession _session;

        public ImportTags(ISession session)
        {
            _session = session;
        }

        public string ProcessTags(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            HashSet<TagData> tagDatas = dataReader.GetTags();
            foreach (TagData tagData in tagDatas)
            {
                string name = tagData.Name.Trim();
                Tag tag =
                    _session.QueryOver<Tag>()
                        .Where(b => b.Name.IsInsensitiveLike(name, MatchMode.Exact))
                        .List().FirstOrDefault();
                if (tag == null)
                {
                    tag = new Tag { Name = name };
                    _session.Transact(session => session.Save(tag));
                }
                nopImportContext.AddEntry(tagData.Id, tag);
            }
            return string.Format("{0} tags processed", tagDatas.Count);
        }
    }
}