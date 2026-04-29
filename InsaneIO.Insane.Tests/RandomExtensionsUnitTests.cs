using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InsaneIO.Insane.Tests
{
    [TestClass]
    public class RandomExtensionsUnitTests
    {
        [TestMethod]
        public void NextBytes_ShouldReturnRequestedSize()
        {
            16u.NextBytes().Should().HaveCount(16);
            0u.NextBytes().Should().BeEmpty();
        }

        [TestMethod]
        public void NextValueRange_ShouldRejectInvalidBounds()
        {
            FluentActions.Invoking(() => 5.NextValue(5)).Should().Throw<ArgumentException>();
            FluentActions.Invoking(() => 10.NextValue(1)).Should().Throw<ArgumentException>();
        }
    }
}
