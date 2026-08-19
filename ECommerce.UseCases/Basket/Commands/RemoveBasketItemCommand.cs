using System;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Basket.Commands;

public sealed record RemoveBasketItemCommand(Guid BuyerId, Guid ProductId) : IRequest<Result<GetBasketResponse>>;