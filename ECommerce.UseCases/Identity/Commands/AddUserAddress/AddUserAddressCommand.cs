using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;

namespace ECommerce.UseCases.Identity.Commands.AddUserAddress;

public sealed record AddUserAddressCommand(
    string Label,
    string RecipientFirstName,
    string RecipientLastName,
    string PhoneNumber,
    string Country,
    string City,
    string Street,
    string PostalCode,
    bool IsDefault = false)
    : IRequest<Result<UserAddressResponse>>;
