using Xunit;

namespace Provausio.Practices.Test.Patterns.Specification
{
    public class OrNotSpecificationTests : SpecificationTest
    {
        [Fact]
        public void IsSatisfiedBy_TrueTrue_IsTrue()
        {
            // arrange
            var spec = GetOrNotSpec(true, true);

            // act
            var x = Test(spec);

            // assert
            Assert.True(x);
        }

        [Fact]
        public void IsSatisfiedBy_TrueFalse_IsTrue()
        {
            // arrange
            var spec = GetOrNotSpec(true, false);

            // act
            var x = Test(spec);

            // assert
            Assert.True(x);
        }

        [Fact]
        public void IsSatisfiedBy_FalseTrue_IsFalse()
        {
            // arrange
            var spec = GetOrNotSpec(false, true);

            // act
            var x = Test(spec);

            // assert
            Assert.False(x);
        }

        [Fact]
        public void IsSatisfiedBy_FalseFalse_IsTrue()
        {
            // arrange
            var spec = GetOrNotSpec(false, false);

            // assert
            var x = Test(spec);

            // assert
            Assert.True(x);
        }
    }
}