using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;
using MrCMS.Website;
using MrCMS.Website.Caching;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using StackExchange.Profiling;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public class MobileFriendlyNavigationService : IMobileFriendlyNavigationService
    {
        private readonly ISession _session;
        private readonly IProcessRootNodes _processRootNodes;
        private readonly IProcessChildNodes _processChildNodes;
        private readonly ICacheManager _cacheManager;
        private readonly Site _site;

        public MobileFriendlyNavigationService(ISession session, IProcessRootNodes processRootNodes, IProcessChildNodes processChildNodes, ICacheManager cacheManager, Site site)
        {
            _session = session;
            _processRootNodes = processRootNodes;
            _processChildNodes = processChildNodes;
            _cacheManager = cacheManager;
            _site = site;
        }

        public List<MobileFriendlyNavigationRootNode> GetRootNodes(Webpage rootWebpage)
        {
            using (MiniProfiler.Current.Step("Get Root Nodes"))
            {
                return _cacheManager.Get(
                    string.Format("mobile-friendly-nav.{0}.{1}", _site.Id, (rootWebpage == null ? "0" : rootWebpage.Id.ToString())), () =>
                    {
                        var rootNodes = _session.QueryOver<Webpage>()
                            .Where(
                                node => node.Parent == rootWebpage && node.RevealInNavigation && node.Published)
                            .OrderBy(node => node.DisplayOrder).Asc
                            .Cacheable()
                            .List();

                        var mobileFriendlyNavigationChildNodes = GetMobileFriendlyNavigationChildNodes(rootNodes);

                        var mobileFriendlyNavigationRootNodes = rootNodes
                            .Select(root => new MobileFriendlyNavigationRootNode
                            {
                                Id = root.Id,
                                Name = root.Name,
                                UrlSegment = root.LiveUrlSegment,
                                Children = GetChildNodeTransforms(mobileFriendlyNavigationChildNodes, root),
                                DocumentType = root.GetType().FullName,
                                DisplayOrder = root.DisplayOrder
                            }).ToList();

                        mobileFriendlyNavigationRootNodes = _processRootNodes.Process(mobileFriendlyNavigationRootNodes);

                        return mobileFriendlyNavigationRootNodes.OrderBy(node => node.DisplayOrder).ToList();
                    }, TimeSpan.FromMinutes(5), CacheExpiryType.Absolute);
            }
        }

        public List<MobileFriendlyNavigationChildNode> GetChildNodes(Webpage parent)
        {
            var nodes = GetMobileFriendlyNavigationChildNodes(new List<Webpage> { parent });
            return GetChildNodeTransforms(nodes, parent);
        }

        private List<MobileFriendlyNavigationChildNode> GetChildNodeTransforms(Dictionary<Webpage, List<MobileFriendlyNavigationChildNode>> mobileFriendlyNavigationChildNodes, Webpage parent)
        {
            if (parent == null)
                return new List<MobileFriendlyNavigationChildNode>();
            var nodes = mobileFriendlyNavigationChildNodes[parent];
            if (nodes.Any() && !(parent is SitemapPlaceholder))
            {
                foreach (var node in nodes)
                    node.DisplayOrder = node.DisplayOrder + 1;
                nodes.Insert(0, new MobileFriendlyNavigationChildNode
                {
                    ChildCount = 0,
                    Id = parent.Id,
                    Name = parent.Name,
                    ParentId = parent.Id,
                    PublishOn = parent.PublishOn,
                    UrlSegment = parent.LiveUrlSegment,
                    DocumentType = parent.GetType().FullName,
                    DisplayOrder = 0
                });
            }
            return _processChildNodes.Process(nodes, parent).OrderBy(node => node.DisplayOrder).ToList();
        }

        private Dictionary<Webpage, List<MobileFriendlyNavigationChildNode>> GetMobileFriendlyNavigationChildNodes(IList<Webpage> parents)
        {
            Webpage webpageAlias = null;
            MobileFriendlyNavigationChildNode nodeAlias = null;

            var countSubNodes = QueryOver.Of<Webpage>()
                .Where(x => x.Parent.Id == webpageAlias.Id && x.RevealInNavigation && x.PublishOn != null)
                .ToRowCountQuery();

            var parentIds = parents.Select(webpage => webpage.Id).ToList();
            var nodes = _session.QueryOver(() => webpageAlias)
                .Where(node => node.RevealInNavigation && node.Published)
                .Where(node => node.Parent.Id.IsIn(parentIds))
                .OrderBy(x => x.DisplayOrder).Asc
                .SelectList(x => x.Select(y => y.Id).WithAlias(() => nodeAlias.Id)
                    .Select(y => y.Parent.Id).WithAlias(() => nodeAlias.ParentId)
                    .Select(y => y.Name).WithAlias(() => nodeAlias.Name)
                    .Select(y => y.UrlSegment).WithAlias(() => nodeAlias.UrlSegment)
                    .Select(y => y.PublishOn).WithAlias(() => nodeAlias.PublishOn)
                    .Select(y => y.DocumentType).WithAlias(() => nodeAlias.DocumentType)
                    .Select(y => y.DisplayOrder).WithAlias(() => nodeAlias.DisplayOrder)
                    .SelectSubQuery(countSubNodes).WithAlias(() => nodeAlias.ChildCount))
                .TransformUsing(Transformers.AliasToBean<MobileFriendlyNavigationChildNode>())
                .Cacheable()
                .List<MobileFriendlyNavigationChildNode>().ToList()
                .GroupBy(node => node.ParentId)
                .ToDictionary(grouping => grouping.Key, g => g.ToList());
            return parents.ToDictionary(webpage => webpage,
                webpage =>
                    nodes.ContainsKey(webpage.Id) ? nodes[webpage.Id] : new List<MobileFriendlyNavigationChildNode>());
        }
    }
}