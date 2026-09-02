using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;

namespace ECommerce.UseCases.Identity.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string? DisplayName) : IRequest<Result<EmailSentResponse>>;
