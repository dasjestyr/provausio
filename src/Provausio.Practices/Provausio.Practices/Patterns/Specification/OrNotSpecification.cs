namespace Provausio.Practices.Patterns.Specification
{
    public class OrNotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public OrNotSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override bool IsSatisfiedBy(T target)
        {
            return
                _left.IsSatisfiedBy(target)
                || _right.IsSatisfiedBy(target) != true;
        }
    }
}