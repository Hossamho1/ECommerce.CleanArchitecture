using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;

namespace ECommerce.UseCases.Identity.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<Result<UserProfileResponse>>;
