using ECommerce.Application.Types.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Types.Queries.Handlers;

public class GetTypeByIdQueryHandler : IRequestHandler<GetTypeByIdQuery, Result<GetByIdTypeResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTypeByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetByIdTypeResponse>> Handle(GetTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var type = await _unitOfWork.Repository<ProductType>().GetByIdAsync(request.Id, cancellationToken);

        if (type is null)
            return Result<GetByIdTypeResponse>.Failure(TypeErrors.NotFound);

        var dto = type.Adapt<GetByIdTypeResponse>();

        return Result<GetByIdTypeResponse>.Success(dto);
    }
}
