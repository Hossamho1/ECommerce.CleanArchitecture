using System;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Basket.Commands;

public sealed record UpdateBasketItemQuantityCommand(Guid BuyerId, Guid ProductId, int Quantity) : IRequest<Result<GetBasketResponse>>;