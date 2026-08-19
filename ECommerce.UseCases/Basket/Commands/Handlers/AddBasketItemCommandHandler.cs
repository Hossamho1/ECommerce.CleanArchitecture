using System.Threading;
using System.Threading.Tasks;
using ECommerce.Application.Basket.Commands;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities; // مسار الـ Product Entity
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Basket.Commands.Handlers;

public class AddBasketItemCommandHandler : IRequestHandler<AddBasketItemCommand, Result<GetBasketResponse>>
{
    private readonly IBasketStore _basketStore;
    private readonly IUnitOfWork _unitOfWork;

    public AddBasketItemCommandHandler(IBasketStore basketStore, IUnitOfWork unitOfWork)
    {
        _basketStore = basketStore;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetBasketResponse>> Handle(AddBasketItemCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
           
            throw new System.Exception("Product not found");
        }

        var basket = await _basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);

  
        var addItemResult = basket.AddItem(
            product.Id,
            product.Name,
            product.PictureUrl,
            product.Price,
            request.Quantity);

        if (addItemResult.IsFailure)
        {
            return Result<GetBasketResponse>.Failure(addItemResult.Error);
        }

        await _basketStore.SaveAsync(basket, cancellationToken);

        var dto = basket.Adapt<GetBasketResponse>();
        return Result<GetBasketResponse>.Success(dto);
    }
}