using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Stubs;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class BulkStockUpdateControllerTests : InMemoryDatabaseTest
    {
        private readonly IBulkStockUpdateAdminService _bulkStockUpdateAdminService;
        private readonly BulkStockUpdateController _bulkStockUpdateController;

        public BulkStockUpdateControllerTests()
        {
            _bulkStockUpdateAdminService = A.Fake<IBulkStockUpdateAdminService>();
            _bulkStockUpdateController = new BulkStockUpdateController(_bulkStockUpdateAdminService);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            ViewResult result = _bulkStockUpdateController.Index();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void IndexPOST_ShouldRedirectToIndex()
        {
            var file = new BasicHttpPostedFileBaseCSV();
            A.CallTo(() => _bulkStockUpdateAdminService.BulkStockUpdate(file.InputStream))
                .Returns(BulkStockUpdateResult.Success());

            RedirectToRouteResult result = _bulkStockUpdateController.Index_POST(file);

            result.RouteValues["action"].Should().Be("Index");
        }
    }
}