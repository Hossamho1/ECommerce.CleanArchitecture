using MediatR;
using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;

namespace ECommerce.UseCases.Identity.Commands.AddUserAddress;

public class AddUserAddressCommandHandler : IRequestHandler<AddUserAddressCommand, Result<UserAddressResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AddUserAddressCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<UserAddressResponse>> Handle(AddUserAddressCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result<UserAddressResponse>.Failure(IdentityErrors.InvalidCredentials);

        // Validate data using domain factory
        var createResult = UserAddress.Create(
            Guid.NewGuid(),
            _currentUser.UserId.Value,
            request.Label,
            request.RecipientFirstName,
            request.RecipientLastName,
            request.PhoneNumber,
            request.Country,
            request.City,
            request.Street,
            request.PostalCode,
            request.IsDefault);

        if (createResult.IsFailure)
            return Result<UserAddressResponse>.Failure(createResult.Error!);

        var address = createResult.Value;

        var repo = _unitOfWork.Repository<UserAddress>();
        repo.Add(address);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserAddressResponse>.Success(new UserAddressResponse
        {
            Id = address.Id,
            UserId = address.UserId,
            Label = string.Empty,
            RecipientName = $"{address.RecipientFirstName} {address.RecipientLastName}",
            Phone = address.PhoneNumber,
            Country = address.Country,
            City = address.City,
            Street = address.Street,
            PostalCode = address.PostalCode
        });
    }
}
