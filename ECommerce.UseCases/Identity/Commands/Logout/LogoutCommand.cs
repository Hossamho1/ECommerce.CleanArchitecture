using MediatR;
using ECommerce.Domain.Common;

namespace ECommerce.UseCases.Identity.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result<object>>;
