using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;

namespace ECommerce.UseCases.Identity.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
