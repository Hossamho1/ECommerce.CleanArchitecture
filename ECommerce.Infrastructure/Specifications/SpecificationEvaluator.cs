using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ECommerce.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T>? specification)
        where T : class
    {
        if (specification is null)
            return inputQuery;

        var query = inputQuery;

        // Apply where predicates
        foreach (var where in specification.WhereExpressions)
            query = query.Where(where);

        // Apply includes
        var includePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var include in specification.Includes)
        {
            var path = GetPropertyPath(include);
            if (!string.IsNullOrEmpty(path))
                includePaths.Add(path);
        }

        foreach (var includeInfo in specification.IncludeExpressions)
        {
            var prev = GetPropertyPath(includeInfo.PreviousExpression);
            var child = GetPropertyPath(includeInfo.LambdaExpression);
            string combined = string.IsNullOrEmpty(prev) ? child : (string.IsNullOrEmpty(child) ? prev : prev + "." + child);
            if (!string.IsNullOrEmpty(combined))
                includePaths.Add(combined);
        }

        foreach (var path in includePaths)
            query = query.Include(path);

        // Apply ordering
        bool appliedAnyOrder = false;
        foreach (var order in specification.OrderExpressions)
        {
            switch (order.OrderType)
            {
                case OrderType.OrderBy:
                    query = Queryable.OrderBy(query, (dynamic)order.KeySelector);
                    appliedAnyOrder = true;
                    break;
                case OrderType.OrderByDescending:
                    query = Queryable.OrderByDescending(query, (dynamic)order.KeySelector);
                    appliedAnyOrder = true;
                    break;
                case OrderType.ThenBy:
                    if (appliedAnyOrder)
                        query = Queryable.ThenBy((IOrderedQueryable<T>)query, (dynamic)order.KeySelector);
                    else
                    {
                        query = Queryable.OrderBy(query, (dynamic)order.KeySelector);
                        appliedAnyOrder = true;
                    }
                    break;
                case OrderType.ThenByDescending:
                    if (appliedAnyOrder)
                        query = Queryable.ThenByDescending((IOrderedQueryable<T>)query, (dynamic)order.KeySelector);
                    else
                    {
                        query = Queryable.OrderByDescending(query, (dynamic)order.KeySelector);
                        appliedAnyOrder = true;
                    }
                    break;
            }
        }

        // Tracking
        if (!specification.IsTrackingEnabled)
            query = query.AsNoTracking();

        // Paging
        if (specification.Skip.HasValue)
            query = query.Skip(specification.Skip.Value);

        if (specification.Take.HasValue)
            query = query.Take(specification.Take.Value);

        return query;
    }

    public static IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> inputQuery, ISpecification<T, TResult>? specification)
        where T : class
        where TResult : class
    {
        if (specification is null)
            return inputQuery.Cast<TResult>();

        var query = GetQuery(inputQuery, (ISpecification<T>?)specification);

        if (specification.Selector != null)
            return query.Select(specification.Selector);

        if (specification.SelectorMany != null)
            return query.SelectMany(specification.SelectorMany);

        // If no selector provided, try to cast
        return query.Cast<TResult>();
    }

    public static IQueryable<T> GetCountQuery<T>(IQueryable<T> inputQuery, ISpecification<T>? specification)
        where T : class
    {
        if (specification is null)
            return inputQuery;

        var query = inputQuery;

        foreach (var where in specification.WhereExpressions)
            query = query.Where(where);

        // includes might affect queries with filters on navigation properties
        var includePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var include in specification.Includes)
        {
            var path = GetPropertyPath(include);
            if (!string.IsNullOrEmpty(path))
                includePaths.Add(path);
        }

        foreach (var includeInfo in specification.IncludeExpressions)
        {
            var prev = GetPropertyPath(includeInfo.PreviousExpression);
            var child = GetPropertyPath(includeInfo.LambdaExpression);
            string combined = string.IsNullOrEmpty(prev) ? child : (string.IsNullOrEmpty(child) ? prev : prev + "." + child);
            if (!string.IsNullOrEmpty(combined))
                includePaths.Add(combined);
        }

        foreach (var path in includePaths)
            query = query.Include(path);

        return query;
    }

    private static string GetPropertyPath(LambdaExpression expression)
    {
        if (expression is null)
            return string.Empty;

        Expression body = expression.Body;
        if (body.NodeType == ExpressionType.Convert || body.NodeType == ExpressionType.ConvertChecked)
            body = ((UnaryExpression)body).Operand;

        var members = new List<string>();
        while (body is MemberExpression memberExpression)
        {
            members.Add(memberExpression.Member.Name);
            body = memberExpression.Expression!;
            if (body is ParameterExpression)
                break;
        }

        members.Reverse();
        return string.Join('.', members);
    }
}
