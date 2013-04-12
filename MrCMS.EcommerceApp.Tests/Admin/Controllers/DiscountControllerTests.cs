using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Admin.Controllers
{
    public class DiscountControllerTests
    {
        private readonly IDiscountManager _discountManager;
        private DiscountController _discountController;

        public DiscountControllerTests()
        {
            _discountManager = A.Fake<IDiscountManager>();
            _discountController = new DiscountController(_discountManager);
        }

        [Fact]
        public void DiscountController_Index_ShouldReturnAViewResult()
        {
            var result = _discountController.Index();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void DiscountController_Index_ShouldCallGetAllOfDiscountManager()
        {
            var result = _discountController.Index();

            A.CallTo(() => _discountManager.GetAll()).MustHaveHappened();
        }

        [Fact]
        public void DiscountController_Index_ShouldReturnTheResultOfGetAllAsTheModel()
        {
            var discounts = new List<Discount>();
            A.CallTo(() => _discountManager.GetAll()).Returns(discounts);

            var result = _discountController.Index();

            result.Model.Should().Be(discounts);
        }

        [Fact]
        public void DiscountController_AddGet_ShouldReturnAPartialViewResult()
        {
            var result = _discountController.Add();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void DiscountController_AddGet_ShouldHaveModelOfTypeDiscount()
        {
            var result = _discountController.Add();

            result.Model.Should().BeOfType<Discount>();
        }

        [Fact]
        public void DiscountController_AddPost_ShouldCallAddOnDiscountManagerWithPassedDiscount()
        {
            var discount = new Discount();

            _discountController.Add_POST(discount);

            A.CallTo(() => _discountManager.Add(discount)).MustHaveHappened();
        }

        [Fact]
        public void DiscountController_AddPost_ShouldReturnRedirectToIndex()
        {
            var discount = new Discount();

            var result = _discountController.Add_POST(discount);

            result.Should().BeOfType<RedirectToRouteResult>();
            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void DiscountController_EditGet_ShouldReturnAViewResult()
        {
            var discount = new Discount();

            var result = _discountController.Edit(discount);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void DiscountController_EditGet_ShouldReturnThePassedDiscountAsTheModel()
        {
            var discount = new Discount();

            var result = _discountController.Edit(discount);

            result.Model.Should().Be(discount);
        }

        [Fact]
        public void DiscountController_EditPost_ShouldCallDiscountManagerSaveWithThePassedArguments()
        {
            var discount = new Discount();
            var discountLimitation = A.Dummy<DiscountLimitation>();
            var discountApplication = A.Dummy<DiscountApplication>();

            _discountController.Edit_POST(discount, discountLimitation, discountApplication);

            A.CallTo(() => _discountManager.Save(discount, discountLimitation, discountApplication)).MustHaveHappened();
        }

        [Fact]
        public void DiscountController_EditPost_ShouldReturnRedirectToIndex()
        {
            var discount = new Discount();
            var discountLimitation = A.Dummy<DiscountLimitation>();
            var discountApplication = A.Dummy<DiscountApplication>();

            var result = _discountController.Edit_POST(discount, discountLimitation, discountApplication);

            result.Should().BeOfType<RedirectToRouteResult>();
            result.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void DiscountController_DeleteGet_ShouldReturnAPartialViewResult()
        {
            var discount = new Discount();

            var result = _discountController.Delete(discount);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void DiscountController_DeleteGet_ShouldReturnPassedObjectAsModel()
        {
            var discount = new Discount();

            var result = _discountController.Delete(discount);

            result.Model.Should().Be(discount);
        }

        [Fact]
        public void DiscountController_DeletePost_ShouldCallDeleteOnThePassedDiscount()
        {
            var discount = new Discount();

            _discountController.Delete_POST(discount);

            A.CallTo(() => _discountManager.Delete(discount)).MustHaveHappened();
        }

        [Fact]
        public void DiscountController_DeletePost_ShouldReturnRedirectToIndex()
        {
            var discount = new Discount();

            var result = _discountController.Delete_POST(discount);
            
            result.Should().BeOfType<RedirectToRouteResult>();
            result.RouteValues["action"].Should().Be("Index");
        }
    }
}