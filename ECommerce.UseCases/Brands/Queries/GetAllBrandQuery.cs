using ECommerce.Application.Brands.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Brands.Queries;

// MediatR request for getting all brands
public sealed record GetAllBrandQuery() : IRequest<Result<IReadOnlyList<GetAllBrandResponse>>>;
