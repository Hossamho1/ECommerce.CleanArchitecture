using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Basket.Commands;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Basket.Commands.Handlers;

public class UpdateBasketItemQuantityCommandHandler : IRequestHandler<UpdateBasketItemQuantityCommand, Result<GetBasketResponse>>
{
    private readonly IBasketStore _basketStore;

    public UpdateBasketItemQuantityCommandHandler(IBasketStore basketStore)
    {
        _basketStore = basketStore;
    }

    public async Task<Result<GetBasketResponse>> Handle(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var basket = await _basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);

        basket.UpdateItemQuantity(request.ProductId, request.Quantity);

        await _basketStore.SaveAsync(basket, cancellationToken);

        var dto = basket.Adapt<GetBasketResponse>();
        return Result<GetBasketResponse>.Success(dto);
    }
}