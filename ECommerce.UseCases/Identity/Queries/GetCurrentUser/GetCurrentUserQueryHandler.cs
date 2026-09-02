using MediatR;
using ECommerce.Domain.Common;
using ECommerce.Domain.Errors;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Common.Interfaces;

namespace ECommerce.UseCases.Identity.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserProfileResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserQueryHandler(
        IIdentityService identityService,
        ICurrentUserService currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<Result<UserProfileResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result<UserProfileResponse>.Failure(IdentityErrors.UserNotFound);

        var res = await _identityService.GetUserByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (!res.IsSuccess)
            return Result<UserProfileResponse>.Failure(res.Error!);

        var dto = new UserProfileResponse
        {
            UserId = res.Value.UserId,
            Email = res.Value.Email,
            DisplayName = res.Value.DisplayName
        };

        return Result<UserProfileResponse>.Success(dto);
    }
}
