using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Utils;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
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

        public MobileFriendlyNavigationService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<MobileFriendlyNavigationRootNode> GetRootNodes(Widgets.MobileFriendlyNavigation widget)
        {
            var baseQuery = _session.QueryOver<Webpage>();
            if (widget.RootWebpage != null && widget.RootWebpage.Id > 0)
            {
                baseQuery = baseQuery.Where(x => x.Parent.Id == widget.RootWebpage.Id);
            }
            else
            {
                baseQuery = baseQuery.Where(x => x.Parent==null);
            }
            IEnumerable<Webpage> rootNodes = baseQuery.Where(node => node.RevealInNavigation && node.PublishOn != null)
                .OrderBy(node => node.DisplayOrder).Asc
                .Cacheable()
                .List()
                .Where(node => node.PublishOn <= CurrentRequestData.Now)
                .ToList();

            return rootNodes
                .Select(root => new MobileFriendlyNavigationRootNode
                {
                    Id = root.Id,
                    Name = root.Name,
                    UrlSegment = root.LiveUrlSegment,
                    Children = GetChildNodeTransforms(root)
                });
        }

        public IEnumerable<MobileFriendlyNavigationChildNode> GetChildNodes(Webpage parent)
        {
            return GetChildNodeTransforms(parent);
        }

        private IEnumerable<MobileFriendlyNavigationChildNode> GetChildNodeTransforms(Webpage parent)
        {
            if (parent == null)
                return new List<MobileFriendlyNavigationChildNode>();
            Webpage webpageAlias = null;
            MobileFriendlyNavigationChildNode nodeAlias = null;

            var countSubNodes = QueryOver.Of<Webpage>()
                .Where(x => x.Parent.Id == webpageAlias.Id && x.RevealInNavigation && x.PublishOn != null)
                .ToRowCountQuery();

            var nodes = _session.QueryOver(() => webpageAlias)
                .Where(node => node.RevealInNavigation && node.PublishOn != null)
                .Where(node => node.Parent.Id == parent.Id)
                .OrderBy(x=>x.DisplayOrder).Asc
                .SelectList(x => x.Select(y => y.Id).WithAlias(() => nodeAlias.Id)
                    .Select(y => y.Parent.Id).WithAlias(() => nodeAlias.ParentId)
                    .Select(y => y.Name).WithAlias(() => nodeAlias.Name)
                    .Select(y => y.UrlSegment).WithAlias(() => nodeAlias.UrlSegment)
                    .Select(y => y.PublishOn).WithAlias(() => nodeAlias.PublishOn)
                    .SelectSubQuery(countSubNodes).WithAlias(() => nodeAlias.ChildCount))
                .TransformUsing(Transformers.AliasToBean<MobileFriendlyNavigationChildNode>())
                .Cacheable()
                .List<MobileFriendlyNavigationChildNode>().ToList()
                .FindAll(root => root.PublishOn <= CurrentRequestData.Now);
            if (nodes.Any())
            {
                if (!(parent is SitemapPlaceholder))
                {
                    nodes.Insert(0, new MobileFriendlyNavigationChildNode
                    {
                        ChildCount = 0,
                        Id = parent.Id,
                        Name = parent.Name,
                        ParentId = parent.Id,
                        PublishOn = parent.PublishOn,
                        UrlSegment = parent.LiveUrlSegment
                    });
                }
            }
            return nodes;
        }
    }
}