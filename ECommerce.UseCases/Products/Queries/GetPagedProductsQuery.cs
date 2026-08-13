using ECommerce.Application.Products.Dtos;
using ECommerce.Application.Products.Enums;
using ECommerce.Domain.Common;
using MediatR;
using System;

namespace ECommerce.Application.Products.Queries;

public sealed record GetPagedProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    Guid? BrandId = null,
    Guid? TypeId = null,
    ProductSortField SortBy = ProductSortField.Name,
    bool SortDescending = false
) : IRequest<Result<PagedResult<GetAllProductResponse>>>;

