namespace ECommerce.UseCases.Identity.Dtos;

public sealed class UserProfileResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = null!;
    public string? DisplayName { get; init; }
}
