using ECommerce.UseCases.Identity.Dtos;
using ECommerce.Domain.Common;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IUserAddressService
{
    Task<IReadOnlyList<UserAddressResponse>> GetAddressesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<UserAddressResponse>> AddAddressAsync(
        Guid userId,
        string label,
        string recipientFirstName,
        string recipientLastName,
        string phoneNumber,
        string country,
        string city,
        string street,
        string postalCode,
        CancellationToken cancellationToken = default);
}
