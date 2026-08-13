using ECommerce.Application.Types.Dtos;
using ECommerce.Application.Types.Enums;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Types.Queries;

public sealed record GetPagedTypesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    TypeSortField SortBy = TypeSortField.Name,
    bool SortDescending = false
) : IRequest<Result<PagedResult<GetAllTypeResponse>>>;
