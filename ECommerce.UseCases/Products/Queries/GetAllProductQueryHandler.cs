using ECommerce.Application.Products.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Products.Queries;

public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, Result<IReadOnlyList<GetAllProductResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<GetAllProductResponse>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Repository<Product>().GetAllAsync(cancellationToken);

        var dto = products.Adapt<IReadOnlyList<GetAllProductResponse>>();

        return Result<IReadOnlyList<GetAllProductResponse>>.Success(dto);
    }
}
