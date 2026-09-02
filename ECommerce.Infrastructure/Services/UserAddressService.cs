using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Common;

namespace ECommerce.Infrastructure.Services;

public sealed class UserAddressService : IUserAddressService
{
    private readonly StoreDbContext _db;

    public UserAddressService(StoreDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<UserAddressResponse>> GetAddressesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var addresses = await _db.UserAddresses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        return addresses.Select(a => new UserAddressResponse
        {
            Id = a.Id,
            UserId = a.UserId,
            Label = "",
            RecipientName = $"{a.RecipientFirstName} {a.RecipientLastName}",
            Phone = a.PhoneNumber,
            Country = a.Country,
            City = a.City,
            Street = a.Street,
            PostalCode = a.PostalCode
        }).ToList();
    }

    public async Task<Result<UserAddressResponse>> AddAddressAsync(
        Guid userId,
        string label,
        string recipientFirstName,
        string recipientLastName,
        string phoneNumber,
        string country,
        string city,
        string street,
        string postalCode,
        CancellationToken cancellationToken = default)
    {
        var res = UserAddress.Create(
            Guid.NewGuid(),
            userId,
            label,
            recipientFirstName,
            recipientLastName,
            phoneNumber,
            country,
            city,
            street,
            postalCode);

        if (!res.IsSuccess)
            return Result<UserAddressResponse>.Failure(res.Error!);

        var entity = res.Value;
        _db.UserAddresses.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var dto = new UserAddressResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Label = string.Empty,
            RecipientName = $"{entity.RecipientFirstName} {entity.RecipientLastName}",
            Phone = entity.PhoneNumber,
            Country = entity.Country,
            City = entity.City,
            Street = entity.Street,
            PostalCode = entity.PostalCode
        };

        return Result<UserAddressResponse>.Success(dto);
    }
}
