using ECommerce.Application.Brands.Dtos;
using ECommerce.Application.Brands.Enums;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Brands.Queries;

public sealed record GetPagedBrandsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    BrandSortField SortBy = BrandSortField.Name,
    bool SortDescending = false
) : IRequest<Result<PagedResult<GetAllBrandResponse>>>;
