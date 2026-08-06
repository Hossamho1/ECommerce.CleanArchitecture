using System.Linq.Expressions;
using ECommerce.Domain.Specifications;

namespace ECommerce.Application.Specifications;

public sealed class OrderedSpecificationBuilder<T> : SpecificationBuilder<T>,
    IOrderedSpecificationBuilder<T>
    where T : class
{
    internal OrderedSpecificationBuilder(Specification<T> specification)
        : base(specification)
    {
    }

    public IOrderedSpecificationBuilder<T> ThenBy(Expression<Func<T, object?>> orderExpression)
    {
        Specification.AddOrder(new OrderExpressionInfo<T>(orderExpression, OrderType.ThenBy));
        return this;
    }

    public IOrderedSpecificationBuilder<T> ThenByDescending(Expression<Func<T, object?>> orderExpression)
    {
        Specification.AddOrder(new OrderExpressionInfo<T>(orderExpression, OrderType.ThenByDescending));
        return this;
    }
}
