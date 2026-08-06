using ECommerce.Application.Products.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Products.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetByIdProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetByIdProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
            return Result<GetByIdProductResponse>.Failure(ProductErrors.NotFound);

        var dto = product.Adapt<GetByIdProductResponse>();

        return Result<GetByIdProductResponse>.Success(dto);
    }
}
