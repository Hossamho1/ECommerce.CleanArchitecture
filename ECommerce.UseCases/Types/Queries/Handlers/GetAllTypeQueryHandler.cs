using ECommerce.Application.Types.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Types.Queries.Handlers;

public class GetAllTypeQueryHandler : IRequestHandler<GetAllTypeQuery, Result<IReadOnlyList<GetAllTypeResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTypeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<GetAllTypeResponse>>> Handle(GetAllTypeQuery request, CancellationToken cancellationToken)
    {
        var types = await _unitOfWork.Repository<ProductType>().GetAllAsync(cancellationToken);

        var dto = types.Adapt<IReadOnlyList<GetAllTypeResponse>>();

        return Result<IReadOnlyList<GetAllTypeResponse>>.Success(dto);
    }
}
