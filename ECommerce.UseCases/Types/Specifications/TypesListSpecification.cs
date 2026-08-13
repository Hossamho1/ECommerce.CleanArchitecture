using ECommerce.Application.Specifications;
using ECommerce.Application.Types.Enums;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Types.Specifications;

public sealed class TypesListSpecification : Specification<ProductType>
{
    public TypesListSpecification(
        string? search = null,
        TypeSortField sortBy = TypeSortField.Name,
        bool sortDescending = false,
        int? skip = null,
        int? take = null)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            Query.Where(t => t.Name.Contains(s));
        }

        Query.AsNoTracking();

        switch (sortBy)
        {
            case TypeSortField.Name:
                if (sortDescending) Query.OrderByDescending(t => t.Name);
                else Query.OrderBy(t => t.Name);
                break;
            default:
                Query.OrderBy(t => t.Name);
                break;
        }

        if (skip.HasValue) Query.Skip(skip.Value);
        if (take.HasValue) Query.Take(take.Value);
    }
}
