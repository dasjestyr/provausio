using Xunit;

namespace Provausio.Practices.Test.Patterns.Specification
{
    public class NotSpecificationTests : SpecificationTest
    {
        [Fact]
        public void IsSatisfiedBy_True_IsFalse()
        {
            // arrange
            var spec = GetNotSpec(true);

            // act
            var x = Test(spec);

            // assert
            Assert.False(x);
        }

        [Fact]
        public void IsSatisfiedBy_False_IsTrue()
        {
            // arrange
            var spec = GetNotSpec(false);

            // act
            var x = Test(spec);

            // assert
            Assert.True(x);
        }
    }
}