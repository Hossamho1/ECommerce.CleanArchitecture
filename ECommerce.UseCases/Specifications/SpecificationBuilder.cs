using ECommerce.Domain.Specifications;
using System.Linq.Expressions;

namespace ECommerce.Application.Specifications;

public class SpecificationBuilder<T> : ISpecificationBuilder<T> where T : class
{
    protected readonly Specification<T> Specification;

    internal SpecificationBuilder(Specification<T> specification)
        => Specification = specification;

    public ISpecificationBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        Specification.AddWhere(predicate);
        return this;
    }

    public IOrderedSpecificationBuilder<T> OrderBy(Expression<Func<T, object?>> orderExpression)
    {
        Specification.AddOrder(new OrderExpressionInfo<T>(orderExpression, OrderType.OrderBy));

        return new OrderedSpecificationBuilder<T>(Specification);
    }

    public IOrderedSpecificationBuilder<T> OrderByDescending(Expression<Func<T, object?>> orderExpression)
    {
        Specification.AddOrder(new OrderExpressionInfo<T>(orderExpression, OrderType.OrderByDescending));

        return new OrderedSpecificationBuilder<T>(Specification);
    }

    public IIncludableSpecificationBuilder<T, TProperty> Include<TProperty>(
        Expression<Func<T, TProperty>> navigation)
    {
        var expression = Specification.AddInclude(navigation);

        return new IncludableSpecificationBuilder<T, TProperty>(
            Specification,
            expression);
    }

    public IIncludableCollectionSpecificationBuilder<T, TElement> Include<TElement>(
        Expression<Func<T, ICollection<TElement>>> navigation)
    {
        var expression = Specification.AddInclude(navigation);

        return new IncludableCollectionSpecificationBuilder<T, TElement>(
            Specification,
            expression);
    }

    public ISpecificationBuilder<T> Skip(int skip)
    {
        Specification.SetSkip(skip);
        return this;
    }

    public ISpecificationBuilder<T> Take(int take)
    {
        Specification.SetTake(take);
        return this;
    }

    public ISpecificationBuilder<T> AsNoTracking()
    {
        Specification.SetNoTracking();
        return this;
    }

    public ISpecificationBuilder<T> AsTracking()
    {
        Specification.SetTracking();
        return this;
    }
}

public sealed class SpecificationBuilder<T, TResult> : ISpecificationBuilder<T, TResult>
     where T : class
    where TResult : class
{
    private readonly Specification<T, TResult> _specification;
    private readonly SpecificationBuilder<T> _builder;

    public SpecificationBuilder(Specification<T, TResult> specification, SpecificationBuilder<T> builder)
    {
        _specification = specification;
        _builder = builder;
           }



    public IIncludableSpecificationBuilder<T, TProperty> Include<TProperty>(
        Expression<Func<T, TProperty>> navigation)
    {
        return _builder.Include(navigation);
    }

    public IIncludableCollectionSpecificationBuilder<T, TElement> Include<TElement>(
    Expression<Func<T, ICollection<TElement>>> navigation)
    {
        return _builder.Include(navigation);
    }

    public IOrderedSpecificationBuilder<T> OrderBy(
        Expression<Func<T, object?>> orderExpression)
    {
        return _builder.OrderBy(orderExpression);
    }

    public IOrderedSpecificationBuilder<T> OrderByDescending(
        Expression<Func<T, object?>> orderExpression)
    {
        return _builder.OrderByDescending(orderExpression);
    }

    public ISpecificationBuilder<T, TResult> Select(
        Expression<Func<T, TResult>> selector)
    {
        _specification.SetSelector(selector);
        return this;
    }

    public ISpecificationBuilder<T, TResult> SelectMany(
        Expression<Func<T, IEnumerable<TResult>>> selector)
    {
        _specification.SetSelectMany(selector);
        return this;
    }


    public ISpecificationBuilder<T, TResult> Where(
       Expression<Func<T, bool>> predicate)
    {
        _builder.Where(predicate);
        return this;
    }

    ISpecificationBuilder<T, TResult> ISpecificationBuilder<T, TResult>.AsNoTracking()
    {
        _builder.AsNoTracking();
        return this;
    }

    ISpecificationBuilder<T, TResult> ISpecificationBuilder<T, TResult>.AsTracking()
    {
         _builder.AsTracking();
        return this;
    }

    ISpecificationBuilder<T, TResult> ISpecificationBuilder<T, TResult>.Skip(int skip)
    {
         _builder.Skip(skip);
         return this;
    }

    ISpecificationBuilder<T, TResult> ISpecificationBuilder<T, TResult>.Take(int take)
    {
        _builder.Take(take);
         return this;
    }
}

