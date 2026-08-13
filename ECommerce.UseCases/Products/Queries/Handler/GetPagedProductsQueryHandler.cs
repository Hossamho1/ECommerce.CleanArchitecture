using ECommerce.Application.Products.Dtos;
using ECommerce.Application.Products.Specifications;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Products.Queries.Handler;

/// <summary>
/// Handler for GetPagedProductsQuery - implements pagination using the Specification Pattern.
/// 
/// Flow:
/// 1. Query parameters are validated by ValidationBehavior (FluentValidation)
/// 2. Handler creates two Product specifications:
///    a) Specification WITHOUT Skip/Take - used for counting total filtered products
///    b) Specification WITH Skip/Take - used for retrieving the paginated items
/// 3. SpecificationEvaluator applies filters, includes, sorting, and pagination to IQueryable
/// 4. EF Core executes OFFSET/FETCH in SQL (database-side pagination)
/// 5. Results are mapped to DTOs using Mapster
/// 6. PagedResult<T> containing Items and TotalCount is returned in Result<T>
/// </summary>
public sealed class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, Result<PagedResult<GetAllProductResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPagedProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the paginated product query with filtering, sorting, and pagination.
    /// </summary>
    /// <remarks>
    /// Specification Pattern Implementation:
    /// 
    /// Step 1: Normalize Input
    /// - Ensures PageNumber >= 1
    /// - Ensures PageSize > 0
    /// 
    /// Step 2: Get Total Count (Filtered, NOT Paged)
    /// - Creates ProductsListSpecification WITHOUT Skip/Take
    /// - Applies: Where (search, brand, type), Include (navigation properties), OrderBy
    /// - Calls repository.CountAsync() which uses SpecificationEvaluator.GetCountQuery()
    /// - GetCountQuery does NOT apply Skip/Take, ensuring accurate total count
    /// - This spec is discarded after counting
    /// 
    /// Step 3: Get Paged Items (Filtered AND Paged)
    /// - Creates ProductsListSpecification WITH Skip/Take
    /// - Skip = (PageNumber - 1) * PageSize
    /// - Take = PageSize
    /// - Applies: Where, Include, OrderBy, Skip, Take
    /// - Calls repository.ListAsync() which uses SpecificationEvaluator.GetQuery()
    /// - GetQuery applies Skip/Take to build OFFSET/FETCH SQL
    /// - EF Core executes pagination in the database
    /// 
    /// Step 4: Map and Return
    /// - Maps Product entities to GetAllProductResponse DTOs
    /// - Returns PagedResult<GetAllProductResponse> with Items and TotalCount
    /// - PagedResult is wrapped in Result<T> for error handling
    /// </remarks>
    public async Task<Result<PagedResult<GetAllProductResponse>>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
    {
        // Step 1: Normalize pagination parameters
        // (Validation of ranges is already done by GetPagedProductsQueryValidator in the pipeline)
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
        var skip = (pageNumber - 1) * pageSize;

        // Step 2: Create specification WITHOUT pagination for counting
        // This will apply Where (filters), Include (navigation properties), OrderBy
        // but NOT Skip/Take, so CountAsync returns the total filtered count
        var countSpec = new ProductsListSpecification(
            search: request.Search,
            brandId: request.BrandId,
            typeId: request.TypeId,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            skip: null,  // No pagination for count
            take: null);

        var totalCount = await _unitOfWork.Repository<Product>().CountAsync(countSpec, cancellationToken);

        // Step 3: Create specification WITH pagination for fetching items
        // This will apply Where, Include, OrderBy, Skip, Take
        // SpecificationEvaluator.GetQuery() will apply Skip/Take to the IQueryable
        // which translates to OFFSET/FETCH in SQL (database-side pagination)
        var pagedSpec = new ProductsListSpecification(
            search: request.Search,
            brandId: request.BrandId,
            typeId: request.TypeId,
            sortBy: request.SortBy,
            sortDescending: request.SortDescending,
            skip: skip,        // Database offset
            take: pageSize);   // Database limit

        var products = await _unitOfWork.Repository<Product>().ListAsync(pagedSpec, cancellationToken);

        // Step 4: Map entities to DTOs
        var dto = products.Adapt<IReadOnlyList<GetAllProductResponse>>();

        // Step 5: Create PagedResult and return
        var paged = new PagedResult<GetAllProductResponse>(dto, totalCount);

        return Result<PagedResult<GetAllProductResponse>>.Success(paged);
    }
}
