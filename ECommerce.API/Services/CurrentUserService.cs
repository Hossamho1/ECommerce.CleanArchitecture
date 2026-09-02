using System.Security.Claims;
using ECommerce.UseCases.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ECommerce.API.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid? UserId
    {
        get
        {
            var sub = _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? _accessor.HttpContext?.User?.FindFirst("sub")?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Email => _accessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
                             ?? _accessor.HttpContext?.User?.FindFirst("email")?.Value;
}
