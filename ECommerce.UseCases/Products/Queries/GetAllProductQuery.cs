using ECommerce.Application.Products.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Products.Queries;

public sealed record GetAllProductQuery() : IRequest<Result<IReadOnlyList<GetAllProductResponse>>>;
