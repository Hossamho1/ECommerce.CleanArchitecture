namespace ECommerce.UseCases.Identity.Dtos;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType = "Bearer");
