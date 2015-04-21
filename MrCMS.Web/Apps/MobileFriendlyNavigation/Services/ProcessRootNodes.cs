using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public class ProcessRootNodes : IProcessRootNodes
    {
        private readonly List<IRootNodeProcessor> _rootNodeProcessors;

        public ProcessRootNodes(IEnumerable<IRootNodeProcessor> rootNodeProcessors)
        {
            _rootNodeProcessors = rootNodeProcessors.OrderBy(processor => processor.Order).ToList();
        }

        public List<MobileFriendlyNavigationRootNode> Process(List<MobileFriendlyNavigationRootNode> nodes)
        {
            return _rootNodeProcessors.Aggregate(nodes,
                (current, rootNodeProcessor) => rootNodeProcessor.Process(current));
        }
    }
}