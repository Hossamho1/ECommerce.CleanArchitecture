using System;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Basket.Commands;

public sealed record ClearBasketCommand(Guid BuyerId) : IRequest<Result<GetBasketResponse>>;