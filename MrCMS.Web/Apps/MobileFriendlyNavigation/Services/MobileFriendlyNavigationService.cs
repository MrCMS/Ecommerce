using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public class MobileFriendlyNavigationService : IMobileFriendlyNavigationService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public MobileFriendlyNavigationService(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public IEnumerable<MobileFriendlyNavigationRootNode> GetRootNodes()
        {
            IEnumerable<Webpage> rootNodes = _session.QueryOver<Webpage>()
                .Where(node => node.Parent == null && node.Site.Id == _site.Id && node.RevealInNavigation && node.PublishOn != null)
                .OrderBy(node => node.DisplayOrder).Asc
                .Cacheable()
                .List()
                .Where(node => node.PublishOn <= CurrentRequestData.Now)
                .ToList();

            int[] parentIds = rootNodes.Select(x => x.Id).ToArray();
            IEnumerable<MobileFriendlyNavigationChildNode> childNodes = GetChildNodeTransforms(parentIds);

            return rootNodes
                .Select(root => new MobileFriendlyNavigationRootNode
                {
                    Name = root.Name,
                    UrlSegment = root.UrlSegment,
                    Children = childNodes.Where(x => x.ParentId == root.Id)
                });
        }

        public IEnumerable<MobileFriendlyNavigationChildNode> GetChildNodes(int parentId)
        {
            return GetChildNodeTransforms(new[] {parentId});
        }

        private IEnumerable<MobileFriendlyNavigationChildNode> GetChildNodeTransforms(int[] parentIds)
        {
            Webpage webpageAlias = null;
            MobileFriendlyNavigationChildNode nodeAlias = null;

            QueryOver<Webpage, Webpage> countSubNodes = QueryOver.Of<Webpage>()
                .Where(x => x.Parent.Id == webpageAlias.Id)
                .ToRowCountQuery();

            return _session.QueryOver(() => webpageAlias)
                .Where(node => node.RevealInNavigation && node.PublishOn != null)
                .Where(node => node.Parent.Id.IsIn(parentIds))
                .SelectList(x => x.Select(y => y.Id).WithAlias(() => nodeAlias.Id)
                    .Select(y => y.Parent.Id).WithAlias(() => nodeAlias.ParentId)
                    .Select(y => y.Name).WithAlias(() => nodeAlias.Name)
                    .Select(y => y.UrlSegment).WithAlias(() => nodeAlias.UrlSegment)
                    .Select(y => y.PublishOn).WithAlias(() => nodeAlias.PublishOn)
                    .SelectSubQuery(countSubNodes).WithAlias(() => nodeAlias.ChildCount))
                .TransformUsing(Transformers.AliasToBean<MobileFriendlyNavigationChildNode>())
                .Cacheable()
                .List<MobileFriendlyNavigationChildNode>()
                .Where(root => root.PublishOn <= CurrentRequestData.Now);
        }
    }
}