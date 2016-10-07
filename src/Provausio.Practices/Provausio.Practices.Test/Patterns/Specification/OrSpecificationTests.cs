using Xunit;

namespace Provausio.Practices.Test.Patterns.Specification
{
    public class OrSpecificationTests : SpecificationTest
    {
        [Fact]
        public void IsSatisifedBy_BothTrue_IsTrue()
        {
            // arrange
            var spec = GetOrSpec(true, true);

            // act
            var x = Test(spec);

            // assert
            Assert.True(x);
        }

        [Fact]
        public void IsSatisfiedBy_LeftTrue_IsTrue()
        {
            // arrange
            var spec = GetOrSpec(true, false);

            // act
            var x = Test(spec);

            // assert
            Assert.True(x);
        }

        [Fact]
        public void IsSatisfiedBy_RightTrue_IsTrue()
        {
            // arrange
            var spec = GetOrSpec(false, true);

            // act
            var x = Test(spec);

            // assert
            Assert.True(x);
        }

        [Fact]
        public void IsSatisfiedBy_BothFalse_IsFalse()
        {
            // arrange
            var spec = GetOrSpec(false, false);

            // act
            var x = Test(spec);

            // assert
            Assert.False(x);
        }
    }
}