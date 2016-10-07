namespace Provausio.Practices.Patterns.Specification
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
}