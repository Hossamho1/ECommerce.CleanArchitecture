using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Basket.Commands;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Basket.Commands.Handlers;

public class MergeBasketCommandHandler : IRequestHandler<MergeBasketCommand, Result<GetBasketResponse>>
{
    private readonly IBasketStore _basketStore;

    public MergeBasketCommandHandler(IBasketStore basketStore)
    {
        _basketStore = basketStore;
    }

    public async Task<Result<GetBasketResponse>> Handle(MergeBasketCommand request, CancellationToken cancellationToken)
    {
        var anonymousBuyerGuid = System.Guid.Parse(request.AnonymousBuyerId);

        var anonymousBasket = await _basketStore.GetOrCreateAsync(anonymousBuyerGuid, cancellationToken);
        var userBasket = await _basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);

        userBasket.MergeFrom(anonymousBasket);

        await _basketStore.SaveAsync(userBasket, cancellationToken); 
        await _basketStore.DeleteAsync(anonymousBuyerGuid, cancellationToken);

        var dto = userBasket.Adapt<GetBasketResponse>();
        return Result<GetBasketResponse>.Success(dto);
    }
}