using System;
using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Web.Apps.Ecommerce.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Metadata
{
    public class CategoryContainerMetadataTests
    {
        [Fact]
        public void CategoryContainerMetaData_ChildrenListType_ShouldBeWhiteList()
        {
            var metadata = GetMetadata();

            metadata.ChildrenListType.Should().Be(ChildrenListType.WhiteList);
        }

        [Fact]
        public void CategoryContainerMetaData_ChildrenList_ShouldBeJustCategorys()
        {
            var metadata = GetMetadata();

            metadata.ChildrenList.Should().BeEquivalentTo(new List<Type> { typeof(Category) });
        }

        [Fact]
        public void CategoryContainerMetaData_ShowChildrenInAdminNav_ShouldBeFalse()
        {
            var metadata = GetMetadata();

            metadata.ShowChildrenInAdminNav.Should().BeFalse();
        }
        
        private static CategoryContainerMetadata GetMetadata()
        {
            return new CategoryContainerMetadata();
        }
    }
}