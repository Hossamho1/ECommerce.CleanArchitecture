using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.Interfaces;

namespace ECommerce.UseCases.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<object>>
{
    private readonly IIdentityService _identityService;

    public ConfirmEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<object>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            return Result<object>.Failure(Error.Validation("ConfirmEmail.InvalidToken", "Confirmation token is required."));

        var res = await _identityService.ConfirmEmailAsync(request.Email, request.Token, cancellationToken);
        if (!res.IsSuccess)
            return Result<object>.Failure(res.Error!);

        return Result<object>.Success(new object());
    }
}
