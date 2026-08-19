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

namespace ECommerce.UseCases.Identity.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwt,
        IUnitOfWork unitOfWork,
        IOptions<JwtSettings> jwtOptions)
    {
        _identityService = identityService;
        _jwt = jwt;
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validate = await _identityService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);
        if (!validate.IsSuccess)
            return Result<AuthResponse>.Failure(validate.Error!);

        var user = validate.Value;

        var roles = await _identityService.GetRolesAsync(user.UserId, cancellationToken);

        var access = _jwt.GenerateToken(user.UserId, user.Email, user.DisplayName, roles);

        // create refresh token and persist
        var validFor = TimeSpan.FromDays(_jwtSettings.RefreshTokenExpirationDays);
        var (refreshEntity, plain) = ECommerce.Domain.Entities.RefreshToken.CreateNew(user.UserId, validFor);

        var repo = _unitOfWork.Repository<ECommerce.Domain.Entities.RefreshToken>();
        repo.Add(refreshEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(access.AccessToken, plain);
        return Result<AuthResponse>.Success(response);
    }
}
