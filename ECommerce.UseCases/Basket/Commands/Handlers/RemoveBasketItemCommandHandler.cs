using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Basket.Commands;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Basket.Commands.Handlers;

public class RemoveBasketItemCommandHandler : IRequestHandler<RemoveBasketItemCommand, Result<GetBasketResponse>>
{
    private readonly IBasketStore _basketStore;

    public RemoveBasketItemCommandHandler(IBasketStore basketStore)
    {
        _basketStore = basketStore;
    }

    public async Task<Result<GetBasketResponse>> Handle(RemoveBasketItemCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);

        basket.DeleteItem(request.ProductId);

        await _basketStore.SaveAsync(basket, cancellationToken); 

        var dto = basket.Adapt<GetBasketResponse>();
        return Result<GetBasketResponse>.Success(dto);
    }
}