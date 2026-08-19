using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Basket.Commands;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Basket.Commands.Handlers;

public class ClearBasketCommandHandler : IRequestHandler<ClearBasketCommand, Result<GetBasketResponse>>
{
    private readonly IBasketStore _basketStore;

    public ClearBasketCommandHandler(IBasketStore basketStore)
    {
        _basketStore = basketStore;
    }

    public async Task<Result<GetBasketResponse>> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        await _basketStore.DeleteAsync(request.BuyerId, cancellationToken);

        var emptyBasket = new GetBasketResponse(request.BuyerId);
        return Result<GetBasketResponse>.Success(emptyBasket);
    }
}