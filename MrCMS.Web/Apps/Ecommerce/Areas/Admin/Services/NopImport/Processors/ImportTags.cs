using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportTags : IImportTags
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public ImportTags(IStatelessSession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public string ProcessTags(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            HashSet<TagData> tagDatas = dataReader.GetTags();
            var site = _session.Get<Site>(_site.Id);
            Dictionary<string, Tag> tags = _session.QueryOver<Tag>()
                .List().ToDictionary(x => x.Name);
            _session.Transact(session =>
            {
                foreach (TagData tagData in tagDatas)
                {
                    string name = tagData.Name.Trim();
                    Tag tag;
                    if (!tags.ContainsKey(name))
                    {
                        tag = new Tag {Name = name};
                        tag.AssignBaseProperties(site);
                        session.Insert(tag);
                    }
                    else
                    {
                        tag = tags[name];
                    }
                    nopImportContext.AddEntry(tagData.Id, tag);
                }
            });
            return string.Format("{0} tags processed", tagDatas.Count);
        }
    }
}