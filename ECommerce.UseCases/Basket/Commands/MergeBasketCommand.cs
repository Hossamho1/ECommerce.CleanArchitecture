using System;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Basket.Commands;

public sealed record MergeBasketCommand(Guid BuyerId, string AnonymousBuyerId) : IRequest<Result<GetBasketResponse>>;