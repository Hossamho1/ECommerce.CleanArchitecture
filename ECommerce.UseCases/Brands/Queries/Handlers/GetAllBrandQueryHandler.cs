using ECommerce.Application.Brands.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Brands.Queries.Handlers;

public class GetAllBrandQueryHandler : IRequestHandler<GetAllBrandQuery, Result<IReadOnlyList<GetAllBrandResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllBrandQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<GetAllBrandResponse>>> Handle(GetAllBrandQuery request, CancellationToken cancellationToken)
    {
        var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync(cancellationToken);

        var dto = brands.Adapt<IReadOnlyList<GetAllBrandResponse>>();

        return Result<IReadOnlyList<GetAllBrandResponse>>.Success(dto);
    }
}
