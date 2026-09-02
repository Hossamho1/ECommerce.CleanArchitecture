using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Identity.Specifications;

namespace ECommerce.UseCases.Identity.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result<object>.Failure(Error.Validation("RefreshToken.Invalid","Refresh token is required."));

        var hash = Domain.Entities.RefreshToken.ComputeHash(request.RefreshToken);
        var spec = new RefreshTokenByHashSpecification(hash);
        var existing = await _unitOfWork.Repository<Domain.Entities.RefreshToken>().FirstOrDefaultAsync(spec, cancellationToken);

        if (existing is null)
            return Result<object>.Success(new object()); // idempotent

        if (!existing.IsRevoked)
        {
            existing.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<object>.Success(new object());
    }
}
