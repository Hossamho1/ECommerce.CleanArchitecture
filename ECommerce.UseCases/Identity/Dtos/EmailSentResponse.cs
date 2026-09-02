namespace ECommerce.UseCases.Identity.Dtos;

public sealed record EmailSentResponse(
    string Email,
    bool VerificationCodeResent,
    string? ConfirmationToken,
    string Message);
