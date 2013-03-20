using FluentAssertions;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Entities
{
    public class ShippingRateComputationMethodTests
    {
        //[Fact]
        //public void ShippingRateComputationMethod_IsValid_WhenNotWeightS()
        //{
        //    var method = new ShippingRateComputationMethod{};

        //    method.IsValid(1).Should().BeTrue();
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_IsValid_WhenWeightFromIsNullAndToIsSetIfPassedValueIsLessThanToReturnTrue()
        //{
        //    var method = new ShippingRateComputationMethod { WeightTo = 2 };

        //    method.IsValid(1).Should().BeTrue();
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_IsValid_WhenWeightFromIsNullAndToIsSetIfPassedValueIsGreaterThanToReturnFalse()
        //{
        //    var method = new ShippingRateComputationMethod { WeightTo = 2 };

        //    method.IsValid(3).Should().BeFalse();
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_IsValid_WhenWeightFromHasValueAndWeightToHasNoValueLessThanFromShouldBeFalse()
        //{
        //    var method = new ShippingRateComputationMethod { WeightFrom = 2 };

        //    method.IsValid(1).Should().BeFalse();
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_IsValid_WhenWeightFromHasValueAndWeightToHasNoValueGreaterThanFromShouldBeTrue()
        //{
        //    var method = new ShippingRateComputationMethod { WeightFrom = 2 };

        //    method.IsValid(3).Should().BeTrue();
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_IsValid_WhenBothWeightHaveValueLessThanWeightShouldBeFalse()
        //{
        //    var method = new ShippingRateComputationMethod { WeightFrom = 2, WeightTo = 3 };

        //    method.IsValid(1).Should().BeFalse();
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_IsValid_WhenBothWeightHaveValueGreaterThanToShouldBeFalse()
        //{
        //    var method = new ShippingRateComputationMethod { WeightFrom = 2, WeightTo = 3 };

        //    method.IsValid(4).Should().BeFalse();
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_IsValid_WhenBothWeightHaveValueInBetweenShouldBeTrue()
        //{
        //    var method = new ShippingRateComputationMethod { WeightFrom = 2, WeightTo = 3 };

        //    method.IsValid(2.5m).Should().BeTrue();
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_GetPrice_IfMethodIsInvalidForWeightReturnNull()
        //{
        //    var method = new ShippingRateComputationMethod { WeightFrom = 2, WeightTo = 3 };

        //    method.GetPrice(new StubShippingCalculationRequest { Weight = 1 }).HasValue.Should().BeFalse();
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_GetPrice_IfMethodIsValidForWeightAndBasePriceOnlyReturnBasePrice()
        //{
        //    var method = new ShippingRateComputationMethod { WeightFrom = 2, WeightTo = 3, BasePrice = 12 };

        //    method.GetPrice(new StubShippingCalculationRequest { Weight = 2.5m }).Should().Be(12);
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_GetPrice_IfMethodIsValidForWeightAndBaseAndPriceToAreSetReturnTheValueInRelationToTheDifference()
        //{
        //    var method = new ShippingRateComputationMethod { WeightFrom = 2m, WeightTo = 3m, BasePrice = 12m, PriceTo = 14m };

        //    method.GetPrice(new StubShippingCalculationRequest { Weight = 2.5m }).Should().Be(13m);
        //}

        //[Fact]
        //public void ShippingRateComputationMethod_GetPrice_IfMethodIsValidForWeightAndBaseAndPriceToAreSetResultShouldBeRounded()
        //{
        //    var method = new ShippingRateComputationMethod { WeightFrom = 2m, WeightTo = 5m, BasePrice = 12m, PriceTo = 14m };

        //    method.GetPrice(new StubShippingCalculationRequest { Weight = 3m }).Should().Be(12.67m);
        //}
    }

    public class StubShippingCalculationRequest : IShippingCalculationRequest
    {
        public decimal Weight { get; set; }
    }
}