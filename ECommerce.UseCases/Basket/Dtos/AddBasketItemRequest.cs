using System;

namespace ECommerce.Application.Basket.Dtos;

public record AddBasketItemRequest(Guid ProductId, int Quantity);