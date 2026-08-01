using ECommerce.Application.Types.Dtos;
using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Types.Queries;
public sealed class GetAllTypesQuery(ITypeQueryService _queryService)
{
    public async Task<Result<IReadOnlyList<GetAllTypeResponse>>> ExecuteAsync()
    {
        var types = await _queryService.GetAllTypesAsync();

        return Result<IReadOnlyList<GetAllTypeResponse>>.Success(types);
    }
}
