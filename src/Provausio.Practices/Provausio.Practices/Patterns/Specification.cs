namespace Provausio.Practices.Patterns
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T target);

        ISpecification<T> And(ISpecification<T> specification);

        ISpecification<T> Or(ISpecification<T> specification);

        ISpecification<T> Not(ISpecification<T> specification);

        ISpecification<T> AndNot(ISpecification<T> right);

        ISpecification<T> OrNot(ISpecification<T> right);
    }

    public abstract class Specification<T> : ISpecification<T>
    {
        public abstract bool IsSatisfiedBy(T target);

        public ISpecification<T> And(ISpecification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        public ISpecification<T> Or(ISpecification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        public ISpecification<T> Not(ISpecification<T> specification)
        {
            return new NotSpecification<T>(this);
        }

        public ISpecification<T> AndNot(ISpecification<T> specification)
        {
            return new AndNotSpecification<T>(this, specification);
        }

        public ISpecification<T> OrNot(ISpecification<T> specification)
        {
            return new OrNotSpecification<T>(this, specification);
        }
    }

    public class AndSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override bool IsSatisfiedBy(T target)
        {
            return _left.IsSatisfiedBy(target)
                && _right.IsSatisfiedBy(target);
        }
    }

    public class OrSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override bool IsSatisfiedBy(T target)
        {
            return _left.IsSatisfiedBy(target)
                   || _right.IsSatisfiedBy(target);
        }
    }

    public class NotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _specification;

        public NotSpecification(ISpecification<T> specification)
        {
            _specification = specification;
        }

        public override bool IsSatisfiedBy(T target)
        {
            return !_specification.IsSatisfiedBy(target);
        }
    }

    public class AndNotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _left;
        private readonly ISpecification<T> _right;

        public AndNotSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override bool IsSatisfiedBy(T target)
        {
            return
                _left.IsSatisfiedBy(target)
                && _right.IsSatisfiedBy(target) != true;
        }
    }

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
