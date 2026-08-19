using ECommerce.Application.Specifications;
using ECommerce.Domain.Entities;

namespace ECommerce.UseCases.Identity.Specifications;

public sealed class RefreshTokenByHashSpecification : Specification<RefreshToken>
{
    public RefreshTokenByHashSpecification(string hash)
    {
        AddWhere(r => r.TokenHash == hash);
    }
}
