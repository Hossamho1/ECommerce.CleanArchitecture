using System;
using ECommerce.Application.Basket.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Basket.Queries;

public sealed record GetBasketQuery(Guid BuyerId) : IRequest<Result<GetBasketResponse>>;