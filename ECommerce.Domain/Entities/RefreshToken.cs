using ECommerce.Domain.Common;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!; // SHA256 hash of token
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public bool IsRevoked => RevokedAt.HasValue;
    public Guid? ReplacedBy { get; private set; }

    private RefreshToken() { }

    public static (RefreshToken entity, string plainToken) CreateNew(Guid userId, TimeSpan validFor)
    {
        var plain = GenerateToken();
        var hash = ComputeHash(plain);
        var now = DateTimeOffset.UtcNow;

        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = hash,
            ExpiresAt = now.Add(validFor),
            CreatedAt = now
        };

        return (entity, plain);
    }

    public void Revoke(Guid? replacedBy = null)
    {
        if (!IsRevoked)
            RevokedAt = DateTimeOffset.UtcNow;

        ReplacedBy = replacedBy;
    }

    public bool IsActive(DateTimeOffset now)
    {
        return !IsRevoked && ExpiresAt > now;
    }

    public static string ComputeHash(string token)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = sha.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }

    public static string GenerateToken(int size = 64)
    {
        var bytes = new byte[size];
        RandomNumberGenerator.Fill(bytes);
        // base64url
        var base64 = Convert.ToBase64String(bytes);
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
}
