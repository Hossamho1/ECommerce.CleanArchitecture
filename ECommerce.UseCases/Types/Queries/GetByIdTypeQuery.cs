using ECommerce.Application.Types.Dtos;
using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Types.Queries;

public sealed class GetByIdTypeQuery(ITypeQueryService _queryService)
{
    public async Task<Result<GetByIdTypeResponse>> ExecuteAsync(Guid id)
    {
        var type = await _queryService.GetByIdTypeAsync(id);

        if (type == null)
        {
            return Result<GetByIdTypeResponse>.Failure(TypeErrors.NotFound);
        }

        return Result<GetByIdTypeResponse>.Success(type);
    }
}
