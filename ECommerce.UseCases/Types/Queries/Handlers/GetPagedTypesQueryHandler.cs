using ECommerce.Application.Types.Dtos;
using ECommerce.Application.Types.Specifications;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;

namespace ECommerce.Application.Types.Queries.Handlers;

/// <summary>
/// Handler for GetPagedTypesQuery - implements pagination using the Specification Pattern.
///
/// Flow:
/// 1. Query parameters are validated by ValidationBehavior (FluentValidation)
/// 2. Handler creates two ProductType specifications:
///    a) Specification WITHOUT Skip/Take - used for counting total filtered types
///    b) Specification WITH Skip/Take - used for retrieving the paginated items
/// 3. SpecificationEvaluator applies filters, sorting, and pagination to IQueryable
/// 4. EF Core executes OFFSET/FETCH in SQL (database-side pagination)
/// 5. Results are mapped to DTOs using Mapster
/// 6. PagedResult&lt;T&gt; containing Items and TotalCount is returned in Result&lt;T&gt;
/// </summary>
public sealed class GetPagedTypesQueryHandler : IRequestHandler<GetPagedTypesQuery, Result<PagedResult<GetAllTypeResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPagedTypesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<GetAllTypeResponse>>> Handle(GetPagedTypesQuery request, CancellationToken cancellationToken)
    {
        // Step 1: Normalize pagination parameters
        // (Validation of ranges is already done by GetPagedTypesQueryValidator in the pipeline)
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        var skip = (pageNumber - 1) * pageSize;

        // Step 2: Create specification WITHOUT pagination for counting
        var countSpec = new TypesListSpecification(
            search: request.Search,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            skip: null,
            take: null);

        var totalCount = await _unitOfWork.Repository<ProductType>().CountAsync(countSpec, cancellationToken);

        // Step 3: Create specification WITH pagination for fetching items
        var pagedSpec = new TypesListSpecification(
            search: request.Search,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            skip: skip,
            take: pageSize);

        var types = await _unitOfWork.Repository<ProductType>().ListAsync(pagedSpec, cancellationToken);

        // Step 4: Map entities to DTOs
        var dto = types.Adapt<IReadOnlyList<GetAllTypeResponse>>();

        // Step 5: Create PagedResult and return
        var paged = new PagedResult<GetAllTypeResponse>(dto, totalCount);

        return Result<PagedResult<GetAllTypeResponse>>.Success(paged);
    }
}
