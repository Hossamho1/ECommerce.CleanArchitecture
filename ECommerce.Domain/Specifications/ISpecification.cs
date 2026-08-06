using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ECommerce.Domain.Specifications;

public interface ISpecification<T> where T : class // Changed 'Where' to 'where'
{
    IReadOnlyList<Expression<Func<T, bool>>> WhereExpressions { get; }

    // include(p => p.Brand)

    IReadOnlyList<Expression<Func<T, object>>> Includes { get; }
     
    IReadOnlyList<IncludeExpressionInfo> IncludeExpressions { get; }
    IReadOnlyList<OrderExpressionInfo<T>> OrderExpressions { get; }


    int? Skip { get; }
    int? Take { get; }
     bool IsPagingEnabled { get; }
    bool IsTrackingEnabled { get; } 
}
public interface ISpecification<T, TResult> : ISpecification<T>
    where T : class
    where TResult : class
{
    Expression<Func<T, TResult>>? Selector { get; }

    Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; }
}