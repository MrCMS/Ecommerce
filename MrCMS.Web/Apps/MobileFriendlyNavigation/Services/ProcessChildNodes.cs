using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public class ProcessChildNodes : IProcessChildNodes
    {
        private readonly List<IChildNodeProcessor> _rootNodeProcessors;

        public ProcessChildNodes(IEnumerable<IChildNodeProcessor> rootNodeProcessors)
        {
            _rootNodeProcessors = rootNodeProcessors.OrderBy(processor => processor.Order).ToList();
        }

        public List<MobileFriendlyNavigationChildNode> Process(List<MobileFriendlyNavigationChildNode> nodes, Webpage parent)
        {
            return _rootNodeProcessors.Aggregate(nodes,
                (current, rootNodeProcessor) => rootNodeProcessor.Process(current, parent));
        }
    }
}