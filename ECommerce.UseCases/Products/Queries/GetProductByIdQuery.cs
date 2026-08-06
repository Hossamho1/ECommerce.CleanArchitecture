using ECommerce.Application.Products.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Products.Queries;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<GetByIdProductResponse>>;
