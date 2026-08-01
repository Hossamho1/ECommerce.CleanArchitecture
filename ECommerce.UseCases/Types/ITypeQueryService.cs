using ECommerce.Application.Types.Dtos;

namespace ECommerce.Application.Types;

public interface ITypeQueryService
{
    Task<IReadOnlyList<GetAllTypeResponse>> GetAllTypesAsync(CancellationToken cancellationToken = default);
    Task<GetByIdTypeResponse?> GetByIdTypeAsync(Guid id, CancellationToken cancellationToken = default);
}
