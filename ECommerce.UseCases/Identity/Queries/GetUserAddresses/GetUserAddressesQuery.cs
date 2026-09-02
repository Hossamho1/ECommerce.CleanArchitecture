using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;

namespace ECommerce.UseCases.Identity.Queries.GetUserAddresses;

public sealed record GetUserAddressesQuery : IRequest<Result<IReadOnlyList<UserAddressResponse>>>;
