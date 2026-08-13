using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Types.Specifications;

public sealed class TypesListSpecification : Specification<ProductType>
{
    public TypesListSpecification()
    {
        Query.OrderBy(t => t.Name)
             .AsNoTracking();
    }
}
