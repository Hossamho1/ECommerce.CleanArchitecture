using ECommerce.Application.Brands.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Brands.Queries;

public class GetByIdBrandQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<GetByIdBrandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetByIdBrandQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetByIdBrandResponse>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        var brand = await _unitOfWork.Repository<ProductBrand>().GetByIdAsync(request.Id, cancellationToken);

        if (brand is null)
            return Result<GetByIdBrandResponse>.Failure(BrandErrors.NotFound);

        var dto = brand.Adapt<GetByIdBrandResponse>();

        return Result<GetByIdBrandResponse>.Success(dto);
    }
}
