using DataCatalog.Common.Extensions;
using Xunit;
using FluentAssertions;

namespace DataCatalog.Api.UnitTests.Extensions
{
    public class EnumExtensionsTests
    {
        [Theory]
        [InlineData(EnumNameToDescriptionTestEnum.Single, "Single")]
        [InlineData(EnumNameToDescriptionTestEnum.OneSplit, "One split")]
        [InlineData(EnumNameToDescriptionTestEnum.ThreeSplitWords, "Three split words")]
        public void EnumNameToDescription_ReturnsExpected(EnumNameToDescriptionTestEnum value, string expected)
        {
            //Arrange

            //Act
            var actual = value.EnumNameToDescription();

            //Assert
            actual.Should().NotBeNull($"because the method must return split value for {value}");
            actual.Should().Be(expected, $"because the method must return split value for {value}");
        }
    }

    public enum EnumNameToDescriptionTestEnum
    {
        Single,
        OneSplit,
        ThreeSplitWords
    }
}
