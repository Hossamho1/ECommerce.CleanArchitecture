using ECommerce.Application.Commons.Models;

namespace ECommerce.Application.Commons.Interfaces;

public interface IJwtTokenGenerator
{
    AccessTokenResult GenerateToken(
        Guid userId,
        string email,
        string? displayName,
        IEnumerable<string> roles);
}