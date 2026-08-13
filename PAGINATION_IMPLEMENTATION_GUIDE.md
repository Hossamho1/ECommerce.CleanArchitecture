# Product Pagination Implementation - Complete Flow Documentation

## Overview

This document explains the complete flow of product pagination in the ECommerce.API project, from the HTTP request through to the database query and back.

---

## Architecture Diagram

```
HTTP Request (GET /api/products/paged?pageNumber=1&pageSize=10&search=...&brandId=...&typeId=...&sortBy=...&sortDescending=...)
	↓
ProductsController.GetPaged()
	↓
MediatR Dispatcher
	↓
ValidationBehavior (Pipeline Behavior #1)
	↓
GetPagedProductsQueryValidator (FluentValidation)
	- Validates: PageNumber >= 1
	- Validates: PageSize between 1-100
	- Validates: Search term <= 100 characters (optional)
	↓
GetPagedProductsQueryHandler
	↓
Step 1: Normalize Parameters
	- Ensure PageNumber >= 1
	- Ensure PageSize > 0
	↓
Step 2: Count Total Filtered Products
	- Create ProductsListSpecification WITHOUT Skip/Take
	- Repository.CountAsync(spec)
	- SpecificationEvaluator.GetCountQuery()
		- Applies: WHERE (filters) + INCLUDE (navigation) + ORDER BY
		- Does NOT apply: Skip/Take
	- EF Core executes: COUNT(*) in SQL with filters
	- Returns: TotalCount
	↓
Step 3: Fetch Paginated Items
	- Create ProductsListSpecification WITH Skip/Take
	- Skip = (PageNumber - 1) * PageSize
	- Take = PageSize
	- Repository.ListAsync(spec)
	- SpecificationEvaluator.GetQuery()
		- Applies: WHERE + INCLUDE + ORDER BY + Skip + Take
	- EF Core executes: OFFSET ... FETCH NEXT ... in SQL
	- Returns: IReadOnlyList<Product>
	↓
Step 4: Map Entities to DTOs
	- Mapster.Adapt<IReadOnlyList<GetAllProductResponse>>()
	↓
Step 5: Create Result
	- Construct PagedResult<GetAllProductResponse>
	- Return Result<PagedResult<GetAllProductResponse>>.Success()
	↓
ProductsController.GetPaged()
	- Extract PagedResult from Result
	- Create PaginationMeta(pageNumber, pageSize, totalCount)
	- Return ApiResponse with data + pagination metadata
	↓
HTTP Response (200 OK)
{
  "success": true,
  "data": [...products...],
  "meta": {
	"traceId": "...",
	"pagination": {
	  "pageNumber": 1,
	  "pageSize": 10,
	  "totalCount": 42,
	  "totalPages": 5,
	  "hasPreviousPage": false,
	  "hasNextPage": true
	}
  }
}
```

---

## Detailed Component Explanations

### 1. GetPagedProductsQuery (CQRS Query)
**File**: `ECommerce.UseCases/Products/Queries/GetPagedProductsQuery.cs`

```csharp
public sealed record GetPagedProductsQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? Search = null,
	Guid? BrandId = null,
	Guid? TypeId = null,
	ProductSortField SortBy = ProductSortField.Name,
	bool SortDescending = false
) : IRequest<Result<PagedResult<GetAllProductResponse>>>;
```

**Purpose**: 
- Encapsulates pagination request parameters
- Inherits `IRequest<T>` to integrate with MediatR
- Generic parameters ensure type safety for response

**Key Points**:
- All parameters are **read-only** (record property)
- Defaults provided for optional parameters
- Strongly typed sort field `ProductSortField` (prevents invalid sort values)

---

### 2. GetPagedProductsQueryValidator (FluentValidation)
**File**: `ECommerce.UseCases/Products/Queries/Validators/GetPagedProductsQueryValidator.cs`

```csharp
public sealed class GetPagedProductsQueryValidator : AbstractValidator<GetPagedProductsQuery>
{
	public GetPagedProductsQueryValidator()
	{
		RuleFor(x => x.PageNumber)
			.GreaterThanOrEqualTo(1)
			.WithMessage("Page number must be >= 1");

		RuleFor(x => x.PageSize)
			.InclusiveBetween(1, 100)
			.WithMessage("Page size must be between 1 and 100");

		RuleFor(x => x.Search)
			.MaximumLength(100)
			.WithMessage("Search cannot exceed 100 chars")
			.When(x => !string.IsNullOrWhiteSpace(x.Search));
	}
}
```

**When is it executed?**
- Automatically invoked by `ValidationBehavior` (MediatR pipeline behavior)
- Runs BEFORE the handler is called
- If validation fails: `ValidationException` is thrown
- Controller catches and returns 400 Bad Request

**Validation Rules**:
| Field | Rule | Purpose |
|-------|------|---------|
| PageNumber | >= 1 | Prevent invalid/zero page numbers |
| PageSize | 1-100 | Balance between performance and UX |
| Search | <= 100 chars | Prevent extremely long search terms that could slow DB |

---

### 3. ValidationBehavior (MediatR Pipeline)
**File**: `ECommerce.UseCases/Behaviors/ValidationBehavior.cs`

```csharp
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		// Auto-discovers all validators for TRequest
		// If validators found, run them concurrently
		// If any fail, throw ValidationException
		// Otherwise, proceed to actual handler
	}
}
```

**Execution Flow**:
1. MediatR dispatcher detects this behavior
2. Before calling handler, `Handle()` method runs
3. Discovers all `IValidator<GetPagedProductsQuery>` instances (FluentValidation finds `GetPagedProductsQueryValidator`)
4. Runs validations in parallel using `Task.WhenAll()`
5. If any validation fails, throws `ValidationException`
6. If all pass, calls `next()` to proceed to the actual handler

**Benefits**:
- Centralizes validation logic
- Decouples validators from handlers
- Automatic validator discovery
- Reusable across all queries/commands

---

### 4. GetPagedProductsQueryHandler (Core Logic)
**File**: `ECommerce.UseCases/Products/Queries/Handler/GetPagedProductsQueryHandler.cs`

#### Step 1: Normalize Pagination Parameters

```csharp
var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
var skip = (pageNumber - 1) * pageSize;
```

**Why?**
- Validator already checked PageNumber >= 1 and PageSize 1-100
- But we normalize as defensive programming
- `skip` calculation: if PageNumber=1 and PageSize=10, then Skip=0 (correct)
- Skip determines SQL `OFFSET` value

#### Step 2: Count Total Filtered Products (WITHOUT Pagination)

```csharp
var countSpec = new ProductsListSpecification(
	search: request.Search,
	brandId: request.BrandId,
	typeId: request.TypeId,
	sortBy: request.SortBy,
	sortDescending: request.SortDescending,
	skip: null,  // ← NO PAGINATION FOR COUNT
	take: null
);

var totalCount = await _unitOfWork.Repository<Product>().CountAsync(countSpec, cancellationToken);
```

**Why Two Specifications?**

We need `TotalCount` to calculate:
- `TotalPages = Math.Ceiling(TotalCount / PageSize)`
- `HasNextPage = PageNumber < TotalPages`
- `HasPreviousPage = PageNumber > 1`

But if we count AFTER applying Skip/Take, we'd only count the current page's results!

**Solution**: 
- First spec: applies ALL filters (search, brand, type) but NO pagination
- CountAsync uses `SpecificationEvaluator.GetCountQuery()` which:
  - Applies WHERE (filters)
  - Applies INCLUDE (navigation properties needed by filters)
  - Does NOT apply Skip/Take
  - Executes: `SELECT COUNT(*) FROM Products WHERE ...`

**Example SQL Generated**:
```sql
SELECT COUNT(*)
FROM Products p
INNER JOIN ProductBrands pb ON p.ProductBrandId = pb.Id
WHERE p.Name LIKE '%search%' OR p.Description LIKE '%search%'
  AND p.ProductBrandId = <guid>
  AND p.ProductTypeId = <guid>
-- No OFFSET/FETCH
-- Result: 42 (total matching products)
```

#### Step 3: Fetch Paginated Items (WITH Pagination)

```csharp
var pagedSpec = new ProductsListSpecification(
	search: request.Search,
	brandId: request.BrandId,
	typeId: request.TypeId,
	sortBy: request.SortBy,
	sortDescending: request.SortDescending,
	skip: skip,        // ← INCLUDES PAGINATION
	take: pageSize
);

var products = await _unitOfWork.Repository<Product>().ListAsync(pagedSpec, cancellationToken);
```

**Key Difference**: This spec HAS `skip` and `take` parameters

**SpecificationEvaluator.GetQuery() Processing**:
1. Applies WHERE conditions (same filters as count spec)
2. Applies INCLUDE for navigation properties
3. Applies ORDER BY (using ProductSortField enum)
4. Applies Skip(skip) ← Translates to OFFSET
5. Applies Take(take) ← Translates to FETCH NEXT

**Example SQL Generated** (for PageNumber=2, PageSize=10):
```sql
SELECT p.*, pb.*, pt.*
FROM Products p
INNER JOIN ProductBrands pb ON p.ProductBrandId = pb.Id
INNER JOIN ProductTypes pt ON p.ProductTypeId = pt.Id
WHERE p.Name LIKE '%search%' OR p.Description LIKE '%search%'
  AND p.ProductBrandId = <guid>
  AND p.ProductTypeId = <guid>
ORDER BY p.Name ASC
OFFSET 10 ROWS       -- Skip first 10 (page 1's 10 results)
FETCH NEXT 10 ROWS   -- Take 10 (page 2's 10 results)
-- Result: rows 11-20
```

**Why Database-Side Pagination?**
- Without OFFSET/FETCH: EF Core would load ALL matching products into memory, then call .Skip(10).Take(10)
- With OFFSET/FETCH: Database returns only page 2's 10 rows
- Massive performance difference for large result sets (millions of products)

#### Step 4: Map Entities to DTOs

```csharp
var dto = products.Adapt<IReadOnlyList<GetAllProductResponse>>();
```

**Mapster Configuration**:
- Automatically maps `Product` → `GetAllProductResponse`
- Includes: ProductBrand.Name, ProductType.Name (because we included them in the spec)
- Already configured in project's Mapster setup

#### Step 5: Create Result and Return

```csharp
var paged = new PagedResult<GetAllProductResponse>(dto, totalCount);
return Result<PagedResult<GetAllProductResponse>>.Success(paged);
```

**PagedResult Structure**:
```csharp
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
```

**Result<T> Pattern**:
- Wraps successful result with `.Success()`
- Replaces exceptions with Result.IsFailure + Error object
- Type-safe error handling

---

### 5. ProductsListSpecification (Specification Pattern)
**File**: `ECommerce.UseCases/Products/Specifications/ProductsListSpecification.cs`

```csharp
public sealed class ProductsListSpecification : Specification<Product>
{
	public ProductsListSpecification(
		string? search = null,
		Guid? brandId = null,
		Guid? typeId = null,
		ProductSortField sortBy = ProductSortField.Name,
		bool sortDescending = false,
		int? skip = null,
		int? take = null)
	{
		// Apply filters
		if (!string.IsNullOrWhiteSpace(search))
		{
			Query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
		}

		if (brandId.HasValue)
			Query.Where(p => p.ProductBrandId == brandId.Value);

		if (typeId.HasValue)
			Query.Where(p => p.ProductTypeId == typeId.Value);

		// Apply includes (eager loading)
		Query.Include(p => p.ProductBrand)
			 .Include(p => p.ProductType)
			 .AsNoTracking();

		// Apply sorting
		switch (sortBy)
		{
			case ProductSortField.Name:
				Query.OrderBy(p => p.Name);
				break;
			case ProductSortField.Price:
				Query.OrderBy(p => p.Price);
				break;
			case ProductSortField.Brand:
				Query.OrderBy(p => p.ProductBrand.Name);
				break;
			case ProductSortField.Type:
				Query.OrderBy(p => p.ProductType.Name);
				break;
		}

		// Apply pagination (optional)
		if (skip.HasValue) Query.Skip(skip.Value);
		if (take.HasValue) Query.Take(take.Value);
	}
}
```

**Specification Pattern Purpose**:
- Encapsulates database query logic
- Reusable across handlers
- Translates to LINQ expressions
- Evaluated by `SpecificationEvaluator` → IQueryable → EF Core

**Why Not Direct EF Core in Handler?**
- Coupling: Handler tightly bound to EF Core patterns
- Testability: Can't easily mock or substitute
- Reusability: Spec can be used by different handlers
- Separation of Concerns: Query logic separate from business logic

---

### 6. SpecificationEvaluator (Execution Engine)
**File**: `ECommerce.Infrastructure/Specifications/SpecificationEvaluator.cs`

**Key Methods**:

#### GetQuery<T>()
Applies EVERYTHING to the IQueryable:
```csharp
foreach (var where in specification.WhereExpressions)
	query = query.Where(where);

foreach (var include in specification.Includes)
	query = query.Include(path);

foreach (var order in specification.OrderExpressions)
	query = query.OrderBy(...);

if (specification.Skip.HasValue)
	query = query.Skip(specification.Skip.Value);

if (specification.Take.HasValue)
	query = query.Take(specification.Take.Value);

return query;
```

Used by: `repository.ListAsync(spec)` for fetching paginated items

#### GetCountQuery<T>()
Applies FILTERS but NOT pagination:
```csharp
foreach (var where in specification.WhereExpressions)
	query = query.Where(where);

foreach (var include in specification.Includes)
	query = query.Include(path);

// DOES NOT APPLY Skip() or Take()

return query;
```

Used by: `repository.CountAsync(spec)` for getting total count

**Why Separate Methods?**
- Count must represent the TOTAL filtered set, not just the current page
- Skip/Take on a count query would break pagination metadata
- SpecificationEvaluator prevents this mistake by having two methods

---

### 7. ProductsController (API Layer)
**File**: `ECommerce.API/Controllers/ProductsControllers.cs`

```csharp
[HttpGet("paged")]
public async Task<ActionResult<ApiResponse<IReadOnlyList<GetAllProductResponse>>>> GetPaged(
	[FromQuery] int pageNumber = 1,
	[FromQuery] int pageSize = 10,
	[FromQuery] string? search = null,
	[FromQuery] Guid? brandId = null,
	[FromQuery] Guid? typeId = null,
	[FromQuery] ProductSortField sortBy = ProductSortField.Name,
	[FromQuery] bool sortDescending = false,
	CancellationToken ct = default)
{
	// Send query through MediatR (triggers ValidationBehavior + Handler)
	var result = await mediator.Send(
		new GetPagedProductsQuery(pageNumber, pageSize, search, brandId, typeId, sortBy, sortDescending),
		ct);

	// Handle failure
	if (result.IsFailure)
		return Problem(result);

	// Extract paged result
	var paged = result.Value;

	// Create pagination metadata for API response
	var pagination = new PaginationMeta(pageNumber, pageSize, paged.TotalCount);
	// PaginationMeta calculates:
	// - TotalPages = ceil(totalCount / pageSize)
	// - HasPreviousPage = pageNumber > 1
	// - HasNextPage = pageNumber < totalPages

	// Return API response with data + pagination
	return Success(paged.Items, null, pagination);
}
```

**Separation of Concerns**:
| Layer | Responsibility |
|-------|-----------------|
| API (Controller) | HTTP binding, response formatting, PaginationMeta creation |
| Application (Handler) | Business logic, validation, pagination logic |
| Domain (Specification) | Query expressions, filters, sorting |
| Infrastructure (EvaluatorRepo) | EF Core translation, database execution |

---

## Dependency Injection Setup

**File**: `ECommerce.UseCases/DependencyInjection.cs`

```csharp
public static IServiceCollection AddUseCases(this IServiceCollection services)
{
	// 1. Register MediatR and auto-discover handlers
	services.AddMediatR(typeof(GetAllBrandQuery).Assembly);

	// 2. Register FluentValidation validators
	services.AddValidatorsFromAssembly(typeof(GetAllBrandQuery).Assembly);

	// 3. Register ValidationBehavior (MediatR pipeline behavior)
	services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

	return services;
}
```

**What This Does**:
1. **AddMediatR**: Scans assembly for all `IRequestHandler<,>` implementations and registers them
   - Finds: `GetPagedProductsQueryHandler`

2. **AddValidatorsFromAssembly**: Scans for all `IValidator<>` implementations
   - Finds: `GetPagedProductsQueryValidator`
   - Registers as Transient (new instance per validation)

3. **AddTransient**: Registers `ValidationBehavior<,>` as a pipeline behavior
   - Executed before EVERY MediatR request
   - ValidationBehavior constructor receives `IEnumerable<IValidator<TRequest>>`
   - For `GetPagedProductsQuery`, injected list contains `GetPagedProductsQueryValidator`

---

## When Each Component Executes

### Request Timeline

**1. HTTP Request Arrives**
```
GET /api/products/paged?pageNumber=1&pageSize=10&search=laptop
```

**2. Controller Action Binds Parameters**
```
pageNumber=1, pageSize=10, search="laptop", brandId=null, etc.
```

**3. MediatR Dispatcher Receives Query**
```csharp
new GetPagedProductsQuery(1, 10, "laptop", null, null, ...)
```

**4. ValidationBehavior Pipeline Intercepts**
- Discovers `GetPagedProductsQueryValidator` (FluentValidation)
- Runs validation rules in parallel
- If valid: proceeds to next step
- If invalid: throws `ValidationException` (caught by error handling middleware)

**5. Handler Logic Executes (if validation passed)**
- Step 1: Normalize pageNumber/pageSize
- Step 2: Create count spec + call repository.CountAsync()
  - Spec with filters but NO pagination
  - SpecificationEvaluator.GetCountQuery() builds IQueryable
  - EF Core executes SQL COUNT
  - Result: totalCount = 42
- Step 3: Create paged spec + call repository.ListAsync()
  - Spec with filters AND pagination
  - SpecificationEvaluator.GetQuery() adds Skip/Take
  - EF Core executes SQL with OFFSET/FETCH
  - Result: 10 Product entities
- Step 4: Map to DTOs (Mapster)
- Step 5: Wrap in PagedResult and Result<T>

**6. Handler Returns Result**
```csharp
Result<PagedResult<GetAllProductResponse>>.Success(
	new PagedResult<GetAllProductResponse>(
		items: [...10 DTOs...],
		totalCount: 42
	)
)
```

**7. Controller Receives Result**
- Unwraps PagedResult
- Creates PaginationMeta(1, 10, 42) - calculates totalPages, hasNextPage, etc.
- Calls `Success(items, null, pagination)`
- Constructs ApiResponse with data + pagination metadata

**8. HTTP Response Sent**
```json
{
  "success": true,
  "data": [...10 products...],
  "meta": {
	"traceId": "xyz123",
	"pagination": {
	  "pageNumber": 1,
	  "pageSize": 10,
	  "totalCount": 42,
	  "totalPages": 5,
	  "hasPreviousPage": false,
	  "hasNextPage": true
	}
  }
}
```

---

## Where Skip() and Take() Are Applied

### In Specification (NOT executed here - just building expressions)
```csharp
if (skip.HasValue) Query.Skip(skip.Value);      // ← Builds expression tree
if (take.HasValue) Query.Take(take.Value);      // ← Builds expression tree
```

### In SpecificationEvaluator (Executed HERE - added to IQueryable)
```csharp
// GetQuery() - Used for fetching items
if (specification.Skip.HasValue)
	query = query.Skip(specification.Skip.Value);   // ← IQueryable now has OFFSET

if (specification.Take.HasValue)
	query = query.Take(specification.Take.Value);   // ← IQueryable now has FETCH NEXT

return query;  // ← IQueryable with all filters + pagination

// GetCountQuery() - Used for counting total
// DOES NOT call Skip() or Take()
return query;  // ← IQueryable with all filters but NO pagination
```

### In EF Core (Ultimately Executed in Database)
```csharp
// What ListAsync executes:
var query = SpecificationEvaluator.GetQuery(dbSet, pagedSpec);
await query.ToListAsync();  // ← Calls DbContext.SaveChanges equivalent
```

Gets translated to SQL:
```sql
SELECT * FROM Products
WHERE ...filters...
ORDER BY ...sort...
OFFSET 10 ROWS
FETCH NEXT 10 ROWS ONLY
```

---

## How TotalCount Is Calculated

### The Problem
If we paged first, then counted:
```sql
-- WRONG: Counts only the paged result
SELECT COUNT(*) FROM Products
OFFSET 10 ROWS
FETCH NEXT 10 ROWS ONLY
-- Returns: 10 (not the total!)
```

### The Solution
Count without pagination:
```sql
-- CORRECT: Counts all filtered products
SELECT COUNT(*) FROM Products
WHERE ...same filters as paged query...
-- Returns: 42 (the actual total!)
```

### Implementation
**In Handler**:
```csharp
// Create spec WITHOUT pagination
var countSpec = new ProductsListSpecification(..., skip: null, take: null);

// Call CountAsync which uses GetCountQuery()
var totalCount = await _unitOfWork.Repository<Product>().CountAsync(countSpec);

// In SpecificationEvaluator.GetCountQuery():
// - Applies WHERE
// - Applies INCLUDE
// - Does NOT apply Skip/Take
// - Returns IQueryable<Product>

// Repository internally calls:
return await query.CountAsync();

// EF Core executes:
SELECT COUNT(*) FROM Products WHERE ...
```

---

## How Validation Works

### Registration (Automatic Discovery)
```csharp
services.AddValidatorsFromAssembly(typeof(GetAllBrandQuery).Assembly);
```

Scans assembly and finds:
- `GetPagedProductsQueryValidator : AbstractValidator<GetPagedProductsQuery>`

Registers as:
- Service: `IValidator<GetPagedProductsQuery>`
- Implementation: `GetPagedProductsQueryValidator`
- Lifetime: Transient

### Execution (Pipeline Behavior)
```csharp
public async Task<TResponse> Handle(TRequest request, ...)
{
	// 1. Get all validators for this request type
	var validators = injected collection;  // Contains GetPagedProductsQueryValidator

	// 2. Validate in parallel
	var results = await Task.WhenAll(
		validators.Select(v => v.ValidateAsync(context))
	);

	// 3. Collect failures
	var failures = results
		.Where(r => r.Errors.Any())
		.SelectMany(r => r.Errors)
		.ToList();

	// 4. Throw if any failures
	if (failures.Any())
		throw new ValidationException(failures);  // ← Caught by middleware, returns 400

	// 5. Otherwise proceed to handler
	return await next();
}
```

### Validation Rules for GetPagedProductsQuery
| Rule | Trigger | Failure |
|------|---------|---------|
| PageNumber >= 1 | PageNumber < 1 | 400 Bad Request |
| PageSize between 1-100 | PageSize < 1 or > 100 | 400 Bad Request |
| Search <= 100 chars | Search length > 100 | 400 Bad Request |

---

## Files Created/Modified

### Created Files
1. **ECommerce.UseCases/Products/Queries/Validators/GetPagedProductsQueryValidator.cs**
   - FluentValidation validator for pagination query

2. **ECommerce.UseCases/Behaviors/ValidationBehavior.cs**
   - MediatR pipeline behavior for automatic validation

### Modified Files
1. **ECommerce.UseCases/ECommerce.Application.csproj**
   - Added: FluentValidation (v11.9.2)
   - Added: FluentValidation.DependencyInjectionExtensions (v11.9.2)

2. **ECommerce.UseCases/DependencyInjection.cs**
   - Added: Validator registration
   - Added: ValidationBehavior registration

3. **ECommerce.UseCases/Products/Queries/Handler/GetPagedProductsQueryHandler.cs**
   - Added: Comprehensive documentation
   - Clarified: specification pattern usage
   - Clarified: Skip/Take application
   - Clarified: TotalCount calculation

### Previously Created (In Previous Phase)
1. **ECommerce.Domain/Common/PagedResult.cs**
   - Result wrapper for paginated data

2. **ECommerce.UseCases/Products/Queries/GetPagedProductsQuery.cs**
   - CQRS Query record

3. **ECommerce.UseCases/Products/Specifications/ProductsListSpecification.cs**
   - Specification with filters + sorting + pagination

4. **ECommerce.UseCases/Products/Queries/Handler/GetPagedProductsQueryHandler.cs**
   - Main handler logic

5. **ECommerce.API/Controllers/ProductsControllers.cs**
   - Added GetPaged() endpoint

---

## Request/Response Example

### Request
```
GET /api/products/paged?pageNumber=2&pageSize=15&search=laptop&brandId=550e8400-e29b-41d4-a716-446655440000&sortBy=Price&sortDescending=false
```

### Query Binding
```csharp
GetPagedProductsQuery(
	PageNumber: 2,
	PageSize: 15,
	Search: "laptop",
	BrandId: new Guid("550e8400-e29b-41d4-a716-446655440000"),
	TypeId: null,
	SortBy: ProductSortField.Price,
	SortDescending: false
)
```

### Validation
```
✓ PageNumber (2) >= 1
✓ PageSize (15) between 1-100
✓ Search ("laptop") <= 100 chars
→ All valid, proceed to handler
```

### Handler Execution
```
1. Normalize: PageNumber=2, PageSize=15, skip=(2-1)*15=15
2. Count total:
   - countSpec = ProductsListSpecification(..., skip: null, take: null)
   - SQL: SELECT COUNT(*) FROM Products 
	 WHERE (Name LIKE '%laptop%' OR Description LIKE '%laptop%')
	   AND ProductBrandId = '550e8400-e29b-41d4-a716-446655440000'
   - Result: totalCount = 247
3. Fetch page 2:
   - pagedSpec = ProductsListSpecification(..., skip: 15, take: 15)
   - SQL: SELECT p.*, pb.* FROM Products p
	 INNER JOIN ProductBrands pb ON p.ProductBrandId = pb.Id
	 WHERE (Name LIKE '%laptop%' OR Description LIKE '%laptop%')
	   AND ProductBrandId = '550e8400-e29b-41d4-a716-446655440000'
	 ORDER BY p.Price ASC
	 OFFSET 15 ROWS
	 FETCH NEXT 15 ROWS ONLY
   - Result: 15 Product entities (rows 16-30)
4. Map: Product → GetAllProductResponse (Mapster)
5. Return: Result<PagedResult<GetAllProductResponse>>.Success(
	 PagedResult(items: [15 DTOs], totalCount: 247)
   )
```

### Controller Response
```csharp
var paged = result.Value;  // PagedResult with 15 items and totalCount=247
var pagination = new PaginationMeta(2, 15, 247);
// Calculates:
// - TotalPages = ceil(247/15) = 17
// - HasPreviousPage = 2 > 1 = true
// - HasNextPage = 2 < 17 = true

return Success(paged.Items, null, pagination);
```

### HTTP Response
```json
{
  "success": true,
  "message": null,
  "data": [
	{
	  "id": "...",
	  "name": "Gaming Laptop Pro",
	  "description": "High-performance laptop",
	  "price": 1299.99,
	  "pictureUrl": "...",
	  "productBrand": "Dell",
	  "productType": "Electronics"
	},
	... 14 more products ...
  ],
  "meta": {
	"traceId": "xyz123abc456",
	"pagination": {
	  "pageNumber": 2,
	  "pageSize": 15,
	  "totalCount": 247,
	  "totalPages": 17,
	  "hasPreviousPage": true,
	  "hasNextPage": true
	}
  }
}
```

Status: **200 OK**

---

## Performance Characteristics

### Database Queries (For pagination)

Page 1, PageSize 10:
```sql
-- Query 1: Count (executed once)
SELECT COUNT(*)
FROM Products p
WHERE ...filters...
-- Time: ~10ms (depends on index on filtered columns)

-- Query 2: Fetch items (executed once)
SELECT p.Id, p.Name, p.Price, pb.Name, pt.Name
FROM Products p
INNER JOIN ProductBrands pb ON p.ProductBrandId = pb.Id
INNER JOIN ProductTypes pt ON p.ProductTypeId = pt.Id
WHERE ...filters...
ORDER BY p.Price ASC
OFFSET 0 ROWS
FETCH NEXT 10 ROWS ONLY
-- Time: ~5ms (OFFSET/FETCH is efficient for all page numbers)

-- Total: 2 queries, ~15ms
```

### Memory Usage
- WITHOUT pagination: Load all 10,000 matching products into memory, then `.Skip(15).Take(10)` → ~40MB
- WITH pagination (our approach): Load only 10 products → ~40KB

**Result**: 1000x less memory for large result sets

### Why Two Queries?
Could we combine them with subqueries? No:
```sql
-- Would this work? NO!
SELECT p.*, pb.*
FROM (SELECT TOP 10 * FROM Products WHERE ... OFFSET 15) p
INNER JOIN ProductBrands pb ON p.ProductBrandId = pb.Id
```

This doesn't give us the COUNT. We need:
1. Count for pagination metadata
2. Paged items for display

Two queries is the optimal approach.

---

## Summary

**Specification Pattern Usage**:
- Encapsulates query logic (where, include, order, skip, take)
- Reusable across different handlers
- Two instances: one for counting (no pagination), one for fetching (with pagination)

**Where Skip/Take Are Applied**:
- Defined in Specification constructor
- Applied in SpecificationEvaluator.GetQuery()
- Translated to SQL OFFSET/FETCH by EF Core
- NOT in-memory after ToListAsync()

**How TotalCount Is Calculated**:
- Separate spec WITHOUT Skip/Take
- SpecificationEvaluator.GetCountQuery() respects this
- SQL COUNT(*) WITH same filters as paged query
- Result is total filtered count, not current page count

**Validation Flow**:
- Request → MediatR Dispatcher
- ValidationBehavior intercepts
- Discovers GetPagedProductsQueryValidator
- Validates PageNumber, PageSize, Search length
- Throws ValidationException if invalid
- Otherwise proceeds to handler

**Overall Architecture**:
- Clean, layered design
- Specification Pattern for reusable queries
- CQRS for clear separation of intent
- FluentValidation + MediatR behaviors for cross-cutting concerns
- Result<T> pattern for error handling
- Automatic validator discovery and pipeline integration

