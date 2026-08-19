using MediatR;
using ECommerce.Domain.Common;

namespace ECommerce.UseCases.Identity.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Token) : IRequest<Result<object>>;
