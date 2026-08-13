using System.Collections.Generic;

namespace ECommerce.Domain.Common;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount);
