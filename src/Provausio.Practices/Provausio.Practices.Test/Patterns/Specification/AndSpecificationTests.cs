using Xunit;

namespace Provausio.Practices.Test.Patterns.Specification
{
    public class AndSpecificationTests : SpecificationTest
    {
        public AndSpecificationTests()
        {
            Target = new FakeTarget();
        }

        [Fact]
        public void IsSatisfiedBy_BothSidesTrue_IsTrue()
        {
            // arrange
            var spec = GetAndSpec(true, true);

            // act
            var x = Test(spec);

            // assert
            Assert.True(x);
        }

        [Fact]
        public void IsSatisfiedBy_LeftTrue_IsFalse()
        {
            // arrange
            var spec = GetAndSpec(true, false);

            // act
            var x = Test(spec);

            // assert
            Assert.False(x);
        }

        [Fact]
        public void IsSatisfiedBy_RightTrue_IsFalse()
        {
            // arrange
            var spec = GetAndSpec(false, true);

            // act
            var x = Test(spec);

            // assert
            Assert.False(x);
        }

        [Fact]
        public void IsSatisfiedBy_BothFalse_IsFalse()
        {
            //a rrange
            var spec = GetAndSpec(false, false);

            // act
            var x = Test(spec);

            // assert
            Assert.False(x);
        }
    }
}
