using ECommerce.Application.Brands.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Brands.Queries;

public sealed record GetBrandByIdQuery(Guid Id) : IRequest<Result<GetByIdBrandResponse>>;
