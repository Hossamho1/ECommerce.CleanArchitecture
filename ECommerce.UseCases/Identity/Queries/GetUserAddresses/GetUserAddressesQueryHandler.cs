using MediatR;
using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Common.Interfaces;

namespace ECommerce.UseCases.Identity.Queries.GetUserAddresses;

public class GetUserAddressesQueryHandler : IRequestHandler<GetUserAddressesQuery, Result<IReadOnlyList<UserAddressResponse>>>
{
    private readonly IUserAddressService _addressService;
    private readonly ICurrentUserService _currentUser;

    public GetUserAddressesQueryHandler(
        IUserAddressService addressService,
        ICurrentUserService currentUser)
    {
        _addressService = addressService;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<UserAddressResponse>>> Handle(GetUserAddressesQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result<IReadOnlyList<UserAddressResponse>>.Failure(IdentityErrors.UserNotFound);

        var addresses = await _addressService.GetAddressesAsync(_currentUser.UserId.Value, cancellationToken);

        return Result<IReadOnlyList<UserAddressResponse>>.Success(addresses);
    }
}
