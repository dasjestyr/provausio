using Xunit;

namespace Provausio.Practices.Test.Patterns.Specification
{
    public class AndNotSpecificationTests : SpecificationTest
    {
        [Fact]
        public void IsSatisfiedBy_TrueTrue_IsFalse()
        {
            // arrange
            var spec = GetAndNotSpec(true, true);

            // act
            var x = Test(spec);

            // assert
            Assert.False(x);
        }

        [Fact]
        public void IsSatisfiedBy_TrueFalse_IsTrue()
        {
            // arrange
            var spec = GetAndNotSpec(true, false);

            // act
            var x = Test(spec);

            // assert
            Assert.True(x);
        }

        [Fact]
        public void IsSatisfiedBy_FalseTrue_IsFalse()
        {
            // arrange
            var spec = GetAndNotSpec(false, true);

            // act
            var x = Test(spec);

            // assert
            Assert.False(x);
        }

        [Fact]
        public void IsSatisfiedBy_FalseFalse_IsFalse()
        {
            // arrage
            var spec = GetAndNotSpec(false, false);

            // act
            var x = Test(spec);

            // assert
            Assert.False(x);
        }
    }
}