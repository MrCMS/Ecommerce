using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class DiscountApplicationControllerTests
    {
        private readonly DiscountApplicationController _discountApplicationController;
        private readonly IDiscountManager _discountManager;

        public DiscountApplicationControllerTests()
        {
            _discountManager = A.Fake<IDiscountManager>();
            _discountApplicationController = new DiscountApplicationController(_discountManager);
        }

        [Fact]
        public void DiscountApplicationController_LoadDiscountApplicationProperties_ShouldCallDiscountManagerGetApplicationWithPassedArguments()
        {
            var discount = new Discount();

            _discountApplicationController.LoadDiscountApplicationProperties(discount, "type");

            A.CallTo(() => _discountManager.GetApplication(discount, "type")).MustHaveHappened();
        }
        
        [Fact]
        public void DiscountApplicationController_LoadDiscountApplicationProperties_ShouldReturnAPartialView()
        {
            var discount = new Discount();

            var result = _discountApplicationController.LoadDiscountApplicationProperties(discount, "type");

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void DiscountApplicationController_LoadDiscountApplicationProperties_ModelShouldBeResultOfServiceCall()
        {
            var discount = new Discount();
            var discountApplication = A.Dummy<DiscountApplication>();
            A.CallTo(() => _discountManager.GetApplication(discount, "type")).Returns(discountApplication);

            var result = _discountApplicationController.LoadDiscountApplicationProperties(discount, "type");

            result.Model.Should().Be(discountApplication);
        }
    }
}