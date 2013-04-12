using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class DiscountLimitationControllerTests
    {
        private readonly DiscountLimitationController _discountLimitationController;
        private readonly IDiscountManager _discountManager;

        public DiscountLimitationControllerTests()
        {
            _discountManager = A.Fake<IDiscountManager>();
            _discountLimitationController = new DiscountLimitationController(_discountManager);
        }

        [Fact]
        public void DiscountLimitationController_LoadDiscountLimitationProperties_ShouldCallDiscountManagerGetLimitationWithPassedArguments()
        {
            var discount = new Discount();

            _discountLimitationController.LoadDiscountLimitationProperties(discount, "type");

            A.CallTo(() => _discountManager.GetLimitation(discount, "type")).MustHaveHappened();
        }
        
        [Fact]
        public void DiscountLimitationController_LoadDiscountLimitationProperties_ShouldReturnAPartialViewIfAModelIsReturned()
        {
            var discount = new Discount();
            var discountLimitation = A.Dummy<DiscountLimitation>();
            A.CallTo(() => _discountManager.GetLimitation(discount, "type")).Returns(discountLimitation);

            var result = _discountLimitationController.LoadDiscountLimitationProperties(discount, "type");

            result.Should().BeOfType<PartialViewResult>();
        }
        [Fact]
        public void DiscountLimitationController_LoadDiscountLimitationProperties_ShouldReturnAnEmptyResultIfNullIsReturned()
        {
            var discount = new Discount();
            A.CallTo(() => _discountManager.GetLimitation(discount, "type")).Returns(null);

            var result = _discountLimitationController.LoadDiscountLimitationProperties(discount, "type");

            result.Should().BeOfType<EmptyResult>();
        }

        [Fact]
        public void DiscountLimitationController_LoadDiscountLimitationProperties_ModelShouldBeResultOfServiceCallIfOneIsReturned()
        {
            var discount = new Discount();
            var discountLimitation = A.Dummy<DiscountLimitation>();
            A.CallTo(() => _discountManager.GetLimitation(discount, "type")).Returns(discountLimitation);

            var result = _discountLimitationController.LoadDiscountLimitationProperties(discount, "type");

            result.As<PartialViewResult>().Model.Should().Be(discountLimitation);
        }
    }
}