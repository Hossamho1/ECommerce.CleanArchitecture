using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Brands.Specifications;

public sealed class BrandsListSpecification : Specification<ProductBrand>
{
    public BrandsListSpecification(
        string? search = null,
        BrandSortField sortBy = BrandSortField.Name,
        bool sortDescending = false,
        int? skip = null,
        int? take = null)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            Query.Where(b => b.Name.Contains(s));
        }

        Query.AsNoTracking();

        switch (sortBy)
        {
            case BrandSortField.Name:
                if (sortDescending) Query.OrderByDescending(b => b.Name);
                else Query.OrderBy(b => b.Name);
                break;
            default:
                Query.OrderBy(b => b.Name);
                break;
        }

        if (skip.HasValue) Query.Skip(skip.Value);
        if (take.HasValue) Query.Take(take.Value);
    }
}
