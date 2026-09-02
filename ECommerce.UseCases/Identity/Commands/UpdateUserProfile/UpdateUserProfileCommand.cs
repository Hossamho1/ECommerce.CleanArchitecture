using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;

namespace ECommerce.UseCases.Identity.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(string? DisplayName) : IRequest<Result<UserProfileResponse>>;
