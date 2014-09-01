using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public interface IIndexSetup
    {
        void ReIndex();
    }

    public class IndexSetup : IIndexSetup
    {
        private readonly IIndexService _indexService;

        public IndexSetup(IIndexService indexService)
        {
            _indexService = indexService;
        }

        public void ReIndex()
        {
            _indexService.InitializeAllIndices();
        }
    }
}