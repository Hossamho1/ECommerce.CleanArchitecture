using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Application.Basket.Queries;
using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Basket.Queries.Handlers;

public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, Result<GetBasketResponse>>
{
    private readonly IBasketStore _basketStore;

    public GetBasketQueryHandler(IBasketStore basketStore)
    {
        _basketStore = basketStore;
    }

    public async Task<Result<GetBasketResponse>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await _basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);

        var dto = basket.Adapt<GetBasketResponse>();

        return Result<GetBasketResponse>.Success(dto);
    }
}