using MrCMS.Web.Apps.Ecommerce.Entities.ETags;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ISetETagService
    {
        void SetETag(ProductVariant productVariant, int eTag);
    }

    public class SetETagService : ISetETagService
    {
        private readonly IETagAdminService _eTagAdminService;

        public SetETagService(IETagAdminService eTagAdminService)
        {
            _eTagAdminService = eTagAdminService;
        }

        public void SetETag(ProductVariant productVariant, int eTag)
        {
            var tag = _eTagAdminService.GetById(eTag);
            if(tag != null)
                productVariant.ETag = tag;
        }
    }
}