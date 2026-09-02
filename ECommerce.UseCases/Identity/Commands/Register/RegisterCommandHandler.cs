using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Common.Interfaces;

namespace ECommerce.UseCases.Identity.Commands.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<EmailSentResponse>>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<EmailSentResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var res = await _identityService.CreateUserAsync(request.Email, request.Password, request.DisplayName, cancellationToken);
        if (!res.IsSuccess)
            return Result<EmailSentResponse>.Failure(res.Error!);
        // Prepare confirmation token (do not send email here — no email service configured)
        var tokenResult = await _identityService.GenerateEmailConfirmationTokenAsync(res.Value.UserId, cancellationToken);

        if (!tokenResult.IsSuccess)
            return Result<EmailSentResponse>.Failure(tokenResult.Error!);

        var token = tokenResult.Value;

        var response = new EmailSentResponse(request.Email, false, token, "Registration successful. Email confirmation is required.");

        return Result<EmailSentResponse>.Success(response);
    }
}
