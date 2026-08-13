using ECommerce.Application.Specifications;
using ECommerce.Application.Products.Enums;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Products.Specifications;

public sealed class ProductsListSpecification : Specification<Product>
{
    public ProductsListSpecification(
        string? search = null,
        Guid? brandId = null,
        Guid? typeId = null,
        ProductSortField sortBy = ProductSortField.Name,
        bool sortDescending = false,
        int? skip = null,
        int? take = null)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            Query.Where(p => p.Name.Contains(s) || p.Description.Contains(s));
        }

        if (brandId.HasValue)
            Query.Where(p => p.ProductBrandId == brandId.Value);

        if (typeId.HasValue)
            Query.Where(p => p.ProductTypeId == typeId.Value);

        Query.Include(p => p.ProductBrand)
             .Include(p => p.ProductType)
             .AsNoTracking();

        // Sorting
        switch (sortBy)
        {
            case ProductSortField.Name:
                if (sortDescending) Query.OrderByDescending(p => p.Name);
                else Query.OrderBy(p => p.Name);
                break;
            case ProductSortField.Price:
                if (sortDescending) Query.OrderByDescending(p => p.Price);
                else Query.OrderBy(p => p.Price);
                break;
            case ProductSortField.Brand:
                if (sortDescending) Query.OrderByDescending(p => p.ProductBrand.Name);
                else Query.OrderBy(p => p.ProductBrand.Name);
                break;
            case ProductSortField.Type:
                if (sortDescending) Query.OrderByDescending(p => p.ProductType.Name);
                else Query.OrderBy(p => p.ProductType.Name);
                break;
            default:
                Query.OrderBy(p => p.Name);
                break;
        }

        if (skip.HasValue) Query.Skip(skip.Value);
        if (take.HasValue) Query.Take(take.Value);
    }
}
