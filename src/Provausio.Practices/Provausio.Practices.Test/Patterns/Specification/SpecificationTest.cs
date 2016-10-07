using Moq;
using Provausio.Practices.Patterns;
using Provausio.Practices.Patterns.Specification;

namespace Provausio.Practices.Test.Patterns.Specification
{
    public class SpecificationTest
    {
        public FakeTarget Target { get; set; }

        public SpecificationTest()
        {
            Target = new FakeTarget();
        }

        protected bool Test(ISpecification<FakeTarget> testSpec)
        {
            return testSpec.IsSatisfiedBy(Target);
        }

        protected ISpecification<FakeTarget> GetSideSpec(bool returns)
        {
            var mock = new Mock<ISpecification<FakeTarget>>();
            mock.Setup(m => m.IsSatisfiedBy(It.IsAny<FakeTarget>())).Returns(returns);
            return mock.Object;
        }

        public AndSpecification<FakeTarget> GetAndSpec(bool left, bool right)
        {
            return new AndSpecification<FakeTarget>(GetSideSpec(left), GetSideSpec(right));
        }

        public OrSpecification<FakeTarget> GetOrSpec(bool left, bool right)
        {
            return new OrSpecification<FakeTarget>(GetSideSpec(left), GetSideSpec(right));
        }

        public NotSpecification<FakeTarget> GetNotSpec(bool target)
        {
            return new NotSpecification<FakeTarget>(GetSideSpec(target));
        }

        public AndNotSpecification<FakeTarget> GetAndNotSpec(bool left, bool right)
        {
            return new AndNotSpecification<FakeTarget>(GetSideSpec(left), GetSideSpec(right));
        }

        public OrNotSpecification<FakeTarget> GetOrNotSpec(bool left, bool right)
        {
            return new OrNotSpecification<FakeTarget>(GetSideSpec(left), GetSideSpec(right));
        }
    }
}