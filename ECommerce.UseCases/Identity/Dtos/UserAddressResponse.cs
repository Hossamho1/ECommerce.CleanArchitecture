namespace ECommerce.UseCases.Identity.Dtos;

public sealed class UserAddressResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Label { get; init; } = null!;
    public string RecipientName { get; init; } = null!;
    public string Phone { get; init; } = null!;
    public string Country { get; init; } = null!;
    public string City { get; init; } = null!;
    public string Street { get; init; } = null!;
    public string PostalCode { get; init; } = null!;
}
