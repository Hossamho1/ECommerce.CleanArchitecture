using MediatR;
using ECommerce.Domain.Common;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.Application.Commons.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using Microsoft.Extensions.Options;
using ECommerce.UseCases.Common.Settings;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.UseCases.Identity.Specifications;

namespace ECommerce.UseCases.Identity.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwt;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IJwtTokenGenerator jwt,
        IOptions<JwtSettings> jwtOptions)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _jwt = jwt;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result<AuthResponse>.Failure(Error.Validation("RefreshToken.Invalid","Refresh token is required."));

        var hash = ECommerce.Domain.Entities.RefreshToken.ComputeHash(request.RefreshToken);
        var spec = new RefreshTokenByHashSpecification(hash);
        var existing = await _unitOfWork.Repository<ECommerce.Domain.Entities.RefreshToken>().FirstOrDefaultAsync(spec, cancellationToken);

        if (existing is null)
            return Result<AuthResponse>.Failure(Error.Unauthorized("RefreshToken.NotFound","Refresh token not found."));

        if (!existing.IsActive(DateTimeOffset.UtcNow))
            return Result<AuthResponse>.Failure(Error.Unauthorized("RefreshToken.Invalid","Refresh token is invalid or expired."));

        // get user and roles
        var userRes = await _identityService.GetUserByIdAsync(existing.UserId, cancellationToken);
        if (!userRes.IsSuccess)
            return Result<AuthResponse>.Failure(userRes.Error!);

        var roles = await _identityService.GetRolesAsync(existing.UserId, cancellationToken);
        var access = _jwt.GenerateToken(existing.UserId, userRes.Value.Email, userRes.Value.DisplayName, roles);

        var validFor = TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays);

        // create new token first
        var (newEntity, plain) = ECommerce.Domain.Entities.RefreshToken.CreateNew(existing.UserId, validFor);

        // revoke existing and set ReplacedBy
        existing.Revoke(newEntity.Id);

        // Add new token and save both changes in one transaction
        var repo = _unitOfWork.Repository<ECommerce.Domain.Entities.RefreshToken>();
        repo.Add(newEntity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(access.AccessToken, plain);
        return Result<AuthResponse>.Success(response);
    }
}
